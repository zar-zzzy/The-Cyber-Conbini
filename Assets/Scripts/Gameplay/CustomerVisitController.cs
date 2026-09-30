using System.Collections;
using CyberConbini.UI;
using UnityEngine;

namespace CyberConbini.Gameplay
{
    public enum CustomerVisitState
    {
        Entering, Shopping, ApproachingCheckout, WaitingAtCheckout,
        CollectingPurchase, Leaving, Completed
    }

    /// <summary>One authored customer visit. The learning flow remains the authority for the sale.</summary>
    [RequireComponent(typeof(Animator))]
    public sealed class CustomerVisitController : MonoBehaviour
    {
        [SerializeField] private Transform customerRoot;
        [SerializeField] private ChallengeFlowController flow;
        [SerializeField] private TerminalUIController terminal;
        [SerializeField] private TerminalExperienceController experience;
        [SerializeField] private Transform[] entryRoute;
        [SerializeField] private Transform[] checkoutRoute;
        [SerializeField] private Transform[] exitRoute;
        [SerializeField] private Transform shelfProduct;
        [SerializeField] private Transform counterProductPoint;
        [SerializeField] private Transform carryPoint;
        [SerializeField] private Transform[] leftDoorParts;
        [SerializeField] private Transform[] rightDoorParts;
        [SerializeField, Min(.1f)] private float walkingSpeed = 1f;
        [SerializeField, Min(1f)] private float turningSpeed = 240f;
        [SerializeField, Min(.1f)] private float doorTravel = 1.02f;
        [SerializeField, Min(.1f)] private float doorDuration = .65f;

        private Animator animator;
        private Vector3[] leftClosed;
        private Vector3[] rightClosed;
        private float handWeight;
        private bool carrying;
        private bool saleReady;
        private bool configured;
        [SerializeField] private bool waitForBegin;
        private bool visitStarted;

        public void BeginVisit() => visitStarted = true;
        public CustomerVisitState State { get; private set; }
        public bool IsReadyForCheckout => State == CustomerVisitState.WaitingAtCheckout;
        public bool IsModuleClosed => State == CustomerVisitState.Completed;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            configured = customerRoot != null && flow != null && terminal != null &&
                shelfProduct != null && counterProductPoint != null && carryPoint != null &&
                ValidRoute(entryRoute) && ValidRoute(checkoutRoute) && ValidRoute(exitRoute) &&
                animator.avatar != null && animator.avatar.isHuman && animator.runtimeAnimatorController != null;
            if (!configured)
            {
                Debug.LogError("Customer visit requires routes, a humanoid animator, product and terminal references.", this);
                enabled = false;
                return;
            }
            leftClosed = CapturePositions(leftDoorParts);
            rightClosed = CapturePositions(rightDoorParts);
            customerRoot.SetPositionAndRotation(entryRoute[0].position, entryRoute[0].rotation);
            animator.applyRootMotion = false;
            terminal.SetVisitInteractionAllowed(false);
        }

        private void OnEnable()
        {
            if (flow != null) flow.AllChallengesCompleted += OnSaleReady;
        }

        private void OnDisable()
        {
            if (flow != null) flow.AllChallengesCompleted -= OnSaleReady;
            StopAllCoroutines();
            handWeight = 0f;
            if (configured && terminal != null && !IsModuleClosed)
                terminal.SetVisitInteractionAllowed(true);
        }

        private void OnSaleReady() => saleReady = true;

        private IEnumerator Start()
        {
            if (!configured) yield break;
            yield return new WaitUntil(() => !waitForBegin || visitStarted);
            yield return null; // The terminal has initialized its challenge presentation.
            terminal.SetVisitStatus("> Un cliente entra en la tienda...");
            yield return new WaitForSeconds(.6f);
            yield return SlideDoor(true);
            yield return Follow(entryRoute, 1);
            yield return SlideDoor(false);
            State = CustomerVisitState.Shopping;
            terminal.SetVisitStatus("> El cliente está eligiendo su compra...");
            yield return Face(shelfProduct.position);
            yield return new WaitForSeconds(.5f);
            yield return Reach(1f, .55f);
            yield return MoveProduct(carryPoint, .6f);
            carrying = true;
            State = CustomerVisitState.ApproachingCheckout;
            yield return Follow(checkoutRoute, 0);
            yield return Face(counterProductPoint.position);
            carrying = false;
            yield return MoveProduct(counterProductPoint, .65f);
            yield return Reach(0f, .4f);
            State = CustomerVisitState.WaitingAtCheckout;
            terminal.SetVisitStatus("> Esperando instrucciones...");
            terminal.SetVisitInteractionAllowed(true);
            yield return new WaitUntil(() => saleReady || flow.AreAllChallengesCompleted);

            State = CustomerVisitState.CollectingPurchase;
            terminal.SetVisitInteractionAllowed(false);
            if (experience != null) experience.SetTerminalEntryAllowed(false);
            // Keep the solution readable before returning to the cashier for the farewell.
            yield return new WaitForSeconds(1.4f);
            if (experience != null)
            {
                while (experience.CurrentState == TerminalExperienceState.EnteringTerminal)
                    yield return null;
                experience.RequestReturnToCashier();
                while (experience.CurrentState == TerminalExperienceState.ReturningToCashier)
                    yield return null;
            }
            yield return new WaitForSeconds(.5f);
            yield return Reach(1f, .55f);
            yield return MoveProduct(carryPoint, .6f);
            carrying = true;
            yield return new WaitForSeconds(.5f);
            State = CustomerVisitState.Leaving;
            // Last two waypoints are outside the doorway; open before crossing it.
            for (int i = 0; i < exitRoute.Length; i++)
            {
                if (i == exitRoute.Length - 2) yield return SlideDoor(true);
                yield return WalkTo(exitRoute[i].position);
            }
            Idle();
            yield return SlideDoor(false);
            State = CustomerVisitState.Completed;
            terminal.ShowVisitCompleted();
            if (experience != null) experience.SetTerminalEntryAllowed(true);
        }

        private void LateUpdate()
        {
            if (carrying && shelfProduct != null && carryPoint != null)
                shelfProduct.SetPositionAndRotation(carryPoint.position, carryPoint.rotation);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!configured) return;
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight);
            if (handWeight > 0f)
            {
                Vector3 position = carrying ? carryPoint.position : shelfProduct.position;
                animator.SetIKPosition(AvatarIKGoal.RightHand, position);
            }
            float lookWeight = State == CustomerVisitState.Shopping ? .65f : .25f;
            animator.SetLookAtWeight(lookWeight, .15f, .7f, .4f, .5f);
            Vector3 lookTarget = State == CustomerVisitState.Shopping
                ? shelfProduct.position
                : customerRoot.position + customerRoot.forward * 2f + Vector3.up * 1.5f;
            animator.SetLookAtPosition(lookTarget);
        }

        private IEnumerator Reach(float target, float duration)
        {
            float start = handWeight;
            for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
            {
                handWeight = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / duration));
                yield return null;
            }
            handWeight = target;
        }

        private IEnumerator MoveProduct(Transform destination, float duration)
        {
            Vector3 start = shelfProduct.position;
            Quaternion rotation = shelfProduct.rotation;
            for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                shelfProduct.position = Vector3.Lerp(start, destination.position, t) +
                    Vector3.up * (Mathf.Sin(t * Mathf.PI) * .08f);
                shelfProduct.rotation = Quaternion.Slerp(rotation, destination.rotation, t);
                yield return null;
            }
            shelfProduct.SetPositionAndRotation(destination.position, destination.rotation);
        }

        private IEnumerator Follow(Transform[] route, int start)
        {
            for (int i = start; i < route.Length; i++) yield return WalkTo(route[i].position);
            Idle();
        }

        private IEnumerator WalkTo(Vector3 destination)
        {
            yield return Face(destination);
            animator.CrossFadeInFixedTime("Walk", .2f);
            while (Vector3.Distance(customerRoot.position, destination) > .005f)
            {
                customerRoot.position = Vector3.MoveTowards(customerRoot.position, destination,
                    Mathf.Max(.1f, walkingSpeed) * Time.deltaTime);
                yield return null;
            }
            customerRoot.position = destination;
            Idle();
        }

        private IEnumerator Face(Vector3 point)
        {
            Vector3 direction = point - customerRoot.position;
            direction.y = 0;
            if (direction.sqrMagnitude < .0001f) yield break;
            Quaternion target = Quaternion.LookRotation(direction);
            float signedAngle = Vector3.SignedAngle(customerRoot.forward, direction, Vector3.up);
            if (Mathf.Abs(signedAngle) > 30f)
                animator.CrossFadeInFixedTime(signedAngle > 0 ? "TurnRight" : "TurnLeft", .15f);
            while (Quaternion.Angle(customerRoot.rotation, target) > .5f)
            {
                customerRoot.rotation = Quaternion.RotateTowards(customerRoot.rotation, target,
                    Mathf.Max(1f, turningSpeed) * Time.deltaTime);
                yield return null;
            }
            customerRoot.rotation = target;
            Idle();
        }

        private void Idle() => animator.CrossFadeInFixedTime("Idle", .15f);

        private IEnumerator SlideDoor(bool open)
        {
            float duration = Mathf.Max(.1f, doorDuration);
            for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                SetDoorPosition(open ? t : 1f - t);
                yield return null;
            }
            SetDoorPosition(open ? 1f : 0f);
        }

        private void SetDoorPosition(float amount)
        {
            for (int i = 0; i < leftDoorParts.Length; i++)
                if (leftDoorParts[i] != null) leftDoorParts[i].position = leftClosed[i] + Vector3.left * (doorTravel * amount);
            for (int i = 0; i < rightDoorParts.Length; i++)
                if (rightDoorParts[i] != null) rightDoorParts[i].position = rightClosed[i] + Vector3.right * (doorTravel * amount);
        }

        private static Vector3[] CapturePositions(Transform[] parts)
        {
            var positions = new Vector3[parts.Length];
            for (int i = 0; i < parts.Length; i++) if (parts[i] != null) positions[i] = parts[i].position;
            return positions;
        }

        private static bool ValidRoute(Transform[] route)
        {
            if (route == null || route.Length == 0) return false;
            foreach (Transform point in route) if (point == null) return false;
            return true;
        }
    }
}
