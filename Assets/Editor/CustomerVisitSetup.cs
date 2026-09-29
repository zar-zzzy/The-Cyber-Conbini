using System;
using System.Collections.Generic;
using System.Linq;
using CyberConbini.Gameplay;
using CyberConbini.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CyberConbini.Editor
{
    /// <summary>Authoring helper: installs only the visit objects and references in the open store.</summary>
    public static class CustomerVisitSetup
    {
        private const string AssetsRoot = "Assets/Art/Models/Props/Onigiri";
        private const string AnimationRoot = "Assets/Kevin Iglesias/Human Animations/Animations/Male/";

        [MenuItem("Cyber-Conbini/Customer Visit/Configure Current Store")]
        public static void Configure()
        {
            if (EditorApplication.isPlaying || SceneManager.GetActiveScene().path != "Assets/Scenes/Conbini_Main.unity")
                throw new InvalidOperationException("Open Conbini_Main in Edit Mode before configuring the visit.");
            if (GameObject.Find("CustomerVisit") != null)
                throw new InvalidOperationException("The visit is already configured. Edit its Inspector references instead.");

            var root = GameObject.Find("Customer").transform;
            var visual = root.Find("Customer_Visual");
            var animator = visual.GetComponent<Animator>();
            var terminal = UnityEngine.Object.FindAnyObjectByType<TerminalUIController>();
            var flow = UnityEngine.Object.FindAnyObjectByType<ChallengeFlowController>();
            var experience = UnityEngine.Object.FindAnyObjectByType<TerminalExperienceController>();
            // Resolve the imported clips before making any scene changes.
            var idle = Clip("Idles/HumanM@Idle01.fbx");
            var walk = Clip("Movement/Walk/HumanM@Walk01_Forward.fbx");
            var left = Clip("Movement/Turn/HumanM@Turn01_Left.fbx");
            var right = Clip("Movement/Turn/HumanM@Turn01_Right.fbx");

            EnsureFolder(AssetsRoot);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Customer_Visit.controller");
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/Customer_Visit.controller");
                var layers = controller.layers;
                layers[0].iKPass = true;
                controller.layers = layers;
                var machine = controller.layers[0].stateMachine;
                AddState(machine, "Idle", idle);
                AddState(machine, "Walk", walk);
                AddState(machine, "TurnLeft", left);
                AddState(machine, "TurnRight", right);
                EditorUtility.SetDirty(controller);
            }

            Undo.RecordObjects(new UnityEngine.Object[] { root, visual, animator }, "Configure customer visit");
            visual.localRotation = Quaternion.identity;
            root.rotation = Quaternion.Euler(0, 180, 0);
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            PrefabUtility.RecordPrefabInstancePropertyModifications(visual);
            PrefabUtility.RecordPrefabInstancePropertyModifications(animator);

            Transform visitRoot = NewObject("CustomerVisit", null, Vector3.zero);
            Transform[] entry =
            {
                Point("01_Outside", visitRoot, new Vector3(-1.2f, 0, 6.4f)),
                Point("02_InsideDoor", visitRoot, new Vector3(-1.2f, 0, 4.1f)),
                Point("03_LeftAisle", visitRoot, new Vector3(-2.25f, 0, 3.7f)),
                Point("04_ProductShelf", visitRoot, new Vector3(-2.45f, 0, 2.6f))
            };
            Transform[] checkout =
            {
                Point("05_ApproachCounter", visitRoot, new Vector3(-2.1f, 0, 1.65f)),
                Point("06_Checkout", visitRoot, new Vector3(-1.38f, 0, 1.08f))
            };
            Transform[] exit =
            {
                Point("07_ReturnAisle", visitRoot, new Vector3(-1.65f, 0, 2.8f)),
                Point("08_ExitApproach", visitRoot, new Vector3(-1.2f, 0, 4.1f)),
                Point("09_ThroughDoor", visitRoot, new Vector3(-1.2f, 0, 5.8f)),
                Point("10_OutsideEnd", visitRoot, new Vector3(-1.2f, 0, 7.1f))
            };
            Transform counter = Point("CounterProductPoint", visitRoot, new Vector3(-1.38f, 1.008f, .4f));
            Transform carry = NewObject("PurchaseCarryPoint", root, Vector3.zero);
            carry.localPosition = new Vector3(.20f, 1.04f, .32f);
            carry.localRotation = Quaternion.identity;
            Transform product = CreateOnigiri(visitRoot);
            product.position = new Vector3(-3.06f, 1.04f, 2.6f);
            product.rotation = Quaternion.Euler(0, 90, 0);

            var visit = Undo.AddComponent<CustomerVisitController>(visual.gameObject);
            var serialized = new SerializedObject(visit);
            Set(serialized, "customerRoot", root);
            Set(serialized, "flow", flow);
            Set(serialized, "terminal", terminal);
            Set(serialized, "experience", experience);
            Set(serialized, "shelfProduct", product);
            Set(serialized, "counterProductPoint", counter);
            Set(serialized, "carryPoint", carry);
            SetArray(serialized, "entryRoute", entry);
            SetArray(serialized, "checkoutRoute", checkout);
            SetArray(serialized, "exitRoute", exit);
            Transform[] all = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<Transform>(true)).ToArray();
            SetArray(serialized, "leftDoorParts", DoorParts(all, 0));
            SetArray(serialized, "rightDoorParts", DoorParts(all, 1));
            serialized.ApplyModifiedProperties();
            var ui = new SerializedObject(terminal);
            Set(ui, "customerReactionTarget", visual); // Nod the visual, never the moving route root.
            ui.ApplyModifiedProperties();
            root.SetPositionAndRotation(entry[0].position, entry[0].rotation);
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            Debug.Log("Customer visit configured: entry, shelf pickup, checkout, departure and existing motion clips.");
        }

        private static Transform[] DoorParts(Transform[] all, int side)
        {
            string panel = side == 0 ? "Door_PanelLeft" : "Door_PanelRight";
            return all.Where(t => t.name == panel || t.name.StartsWith($"SlidingLeaf_Stile_{side}_") ||
                t.name.StartsWith($"SlidingLeaf_Rail_{side}_") || t.name == $"Safety_Band_{side}" ||
                t.name == $"Door_Caution_{side}" || t.name == $"AutoDoor_Mark_{side}").ToArray();
        }

        private static Transform CreateOnigiri(Transform parent)
        {
            var rice = Material("Rice", new Color(.91f, .88f, .76f));
            var nori = Material("Nori", new Color(.045f, .075f, .043f));
            var label = Material("SalmonLabel", new Color(.86f, .36f, .24f));
            string meshPath = AssetsRoot + "/Onigiri_Rice.asset";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (mesh == null)
            {
                mesh = new Mesh { name = "Onigiri_Rice" };
                Vector3[] points =
                {
                    new Vector3(-.115f, 0, -.05f), new Vector3(.115f, 0, -.05f), new Vector3(0, .23f, -.05f),
                    new Vector3(-.115f, 0, .05f), new Vector3(.115f, 0, .05f), new Vector3(0, .23f, .05f)
                };
                int[] faces = { 0,2,1, 3,4,5, 0,3,5, 0,5,2, 1,2,5, 1,5,4, 0,1,4, 0,4,3 };
                mesh.vertices = faces.Select(i => points[i]).ToArray();
                mesh.triangles = Enumerable.Range(0, faces.Length).ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                AssetDatabase.CreateAsset(mesh, meshPath);
            }
            Transform product = NewObject("Visit_Onigiri", parent, Vector3.zero);
            var riceObject = NewObject("Rice", product, Vector3.zero).gameObject;
            riceObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            riceObject.AddComponent<MeshRenderer>().sharedMaterial = rice;
            Cube("Nori_Wrap", product, new Vector3(0, .045f, 0), new Vector3(.085f, .10f, .106f), nori);
            Cube("Salmon_Label", product, new Vector3(0, .145f, -.052f), new Vector3(.065f, .026f, .004f), label);
            return product;
        }

        private static void Cube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(go, "Create purchase prop");
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            go.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static Material Material(string name, Color color)
        {
            string path = AssetsRoot + "/Mat_Onigiri_" + name + ".mat";
            var result = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (result != null) return result;
            // Reuse the renderer's installed URP shader rather than changing the pipeline.
            Shader shader = GameObject.Find("Counter_Desk").GetComponent<Renderer>().sharedMaterial.shader;
            result = new Material(shader) { name = "Mat_Onigiri_" + name };
            result.SetColor("_BaseColor", color);
            result.SetFloat("_Smoothness", .15f);
            AssetDatabase.CreateAsset(result, path);
            return result;
        }

        private static AnimationClip Clip(string relativePath)
        {
            var clip = AssetDatabase.LoadAllAssetsAtPath(AnimationRoot + relativePath)
                .OfType<AnimationClip>().FirstOrDefault(c => !c.name.StartsWith("__preview__"));
            if (clip == null) throw new InvalidOperationException("Missing imported animation: " + relativePath);
            return clip;
        }

        private static void AddState(AnimatorStateMachine machine, string name, AnimationClip clip)
        {
            var state = machine.AddState(name);
            state.motion = clip;
            state.iKOnFeet = true;
            if (name == "Idle") machine.defaultState = state;
        }

        private static Transform Point(string name, Transform parent, Vector3 position)
        {
            var point = NewObject(name, parent, position);
            point.rotation = Quaternion.Euler(0, 180, 0);
            return point;
        }

        private static Transform NewObject(string name, Transform parent, Vector3 position)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create customer visit");
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            return go.transform;
        }

        private static void Set(SerializedObject target, string field, UnityEngine.Object value)
            => target.FindProperty(field).objectReferenceValue = value;

        private static void SetArray(SerializedObject target, string field, Transform[] values)
        {
            var property = target.FindProperty(field);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            EnsureFolder(path.Substring(0, slash));
            AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
        }
    }
}
