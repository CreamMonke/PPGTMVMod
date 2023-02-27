using UnityEngine;

namespace Mod
{
    public class Mod
    {
        public static void Main()
        {
            /*
            Transform debugger = new GameObject().transform;
            debugger.gameObject.AddComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
            debugger.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
            debugger.GetComponent<SpriteRenderer>().sortingOrder=100;
            debugger.parent = door.transform;
            debugger.localPosition = door.anchor;

            fork.transform.localScale=new Vector3(direction, 1f, 1f);

            JointAngleLimits2D zero = new JointAngleLimits2D();
            zero.min = 0f;
            zero.max = 0f;

            Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");

            JointMotor2D jm = new JointMotor2D();
            jm.motorSpeed=-0.05f;
            jm.maxMotorTorque=1000f;
            launcher.motor=jm;
            launcher.useMotor=true;
            */

            #region Military
            #region Grom-2
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Grom-2",
                    DescriptionOverride = "Shoots Missiles.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Military/Grom2/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Military/Grom2/";
                        int wheels = 5;
                        float massMult = 6f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //LAUNCHER
                        HingeJoint2D launcher = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-2f*direction, -2.75f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        launcher.GetComponent<Rigidbody2D>().angularDrag = 9999999999999f;
                        launcher.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        launcher.transform.localScale=new Vector3(direction, 1f, 1f);
                        launcher.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "launcher.png");
                        launcher.gameObject.FixColliders();
                        launcher.connectedBody = rb;
                        launcher.anchor = new Vector2(-3f, -0.5f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction<0?0:-35f;
                        jal.max = direction<0?35:0f;
                        launcher.limits = jal;
                        JointMotor2D jm = new JointMotor2D();
                        jm.motorSpeed=-0.05f;
                        jm.maxMotorTorque=1000f;
                        launcher.motor=jm;
                        launcher.useMotor=true;

                        JointAngleLimits2D jZero = new JointAngleLimits2D();
                        jZero.min = 0f;
                        jZero.max = 0f;
                        HingeJoint2D rocketLauncher = GameObject.Instantiate(ModAPI.FindSpawnable("Rocket Launcher").Prefab, launcher.transform.position + new Vector3(3.5f*direction, 0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        rocketLauncher.transform.localScale = new Vector3(direction, 1f, 1f);
                        rocketLauncher.gameObject.layer=0;//no collision
                        rocketLauncher.GetComponent<SpriteRenderer>().sprite=null;
                        rocketLauncher.connectedBody=launcher.GetComponent<Rigidbody2D>();
                        rocketLauncher.limits=jZero;
                        rocketLauncher.transform.parent=launcher.transform;// will be activated when the the launcher is

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = {new Vector2(-4.85f, -4.3f), new Vector2(-2f, -4.3f), new Vector2(-0.25f, -4.3f), new Vector2(2.85f, -4.3f), new Vector2(5.65f, -4.3f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels+3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.5f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;

                            foreach (Collider2D c in launcher.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(wheel.GetComponent<Collider2D>(), c, true);
                            }
                        }

                        vehicle.objects[wheels]=launcher.gameObject;

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -2750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(8.5f, -3f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels+1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(6f * direction, -2.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.25f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels+2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region HMMWV
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "HMMWV",
                    DescriptionOverride = "This acronym is way too long.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Military/HMMWV/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Military/HMMWV/";
                        int wheels = 2;
                        float massMult = 2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //GUN
                        HingeJoint2D gun = GameObject.Instantiate(ModAPI.FindSpawnable("Light Machine Gun").Prefab, Instance.transform.position + new Vector3(1f * direction, 1.9f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun.transform.localScale = new Vector3(direction, 1f, 1f);
                        gun.connectedBody = rb;
                        gun.anchor = new Vector2(-0.2f, -0.1f);
                        gun.breakForce = 10000f;

                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -30 : -45f;
                        jal.max = direction < 0 ? 45 : 30f;
                        gun.limits = jal;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.25f, -1f), new Vector2(2.65f, -1f)};

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 4];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.3f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        vehicle.objects[wheels] = gun.gameObject;

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex=1;
                        car.MotorSpeed = -2750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-0.25f, -0.2f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(1.1f, -0.2f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 2] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.25f * direction, -0.35f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 3] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region LAV-25
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "LAV-25",
                    DescriptionOverride = "The 25th LAV.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Military/LAV25/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Military/LAV25/";
                        int wheels = 4;
                        float massMult = 2.5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //GUN
                        HingeJoint2D gun = GameObject.Instantiate(ModAPI.FindSpawnable("Detached 30mm HEAT Cannon").Prefab, Instance.transform.position + new Vector3(1.2f * direction, 1.6f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun.GetComponent<Rigidbody2D>().angularDrag = 1000000f;
                        gun.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        gun.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"gun.png");
                        gun.transform.localScale = new Vector3(direction, 1f, 1f);
                        gun.connectedBody = rb;
                        gun.anchor = new Vector2(-1f, 0f);

                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -10 : -50f;
                        jal.max = direction < 0 ? 50 : 10f;
                        gun.limits = jal;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3f, -1.33f), new Vector2(-1f, -1.33f), new Vector2(1f, -1.33f), new Vector2(3f, -1.33f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.75f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        vehicle.objects[wheels] = gun.gameObject;

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -2750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(1f * direction, 0.35f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Military Jeep
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Military Jeep",
                    DescriptionOverride = "A Jeep in the military.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Military/MilitaryJeep/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Military/MilitaryJeep/";
                        int wheels = 2;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        rb.mass = 2000f;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-1.55f, -0.8f), new Vector2(1.9f, -0.8f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 1];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.2f;
                            wheel.GetComponent<Rigidbody2D>().mass = 25f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex=1;
                        car.MotorSpeed = -3200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(1.5f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region KFZ250
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "KFZ 250",
                    DescriptionOverride = "It has tracks and a wheel.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Military/KFZ250/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Military/KFZ250/";
                        int wheels = 9;
                        float massMult = 3f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //GUN
                        HingeJoint2D gun = GameObject.Instantiate(ModAPI.FindSpawnable("Soviet Submachine Gun").Prefab, Instance.transform.position + new Vector3(0.1f * direction, 1.55f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun.transform.localScale = new Vector3(direction, 1f, 1f)*1.2f;
                        gun.connectedBody = rb;
                        gun.anchor = new Vector2(-0.1f, 0f);
                        gun.breakForce = 10000f;

                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -10 : -55f;
                        jal.max = direction < 0 ? 55 : 10f;
                        gun.limits = jal;

                        //GUN2
                        HingeJoint2D gun2 = GameObject.Instantiate(ModAPI.FindSpawnable("Soviet Submachine Gun").Prefab, Instance.transform.position + new Vector3(-3.7f * direction, 1.45f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun2.transform.localScale = new Vector3(-direction, 1f, 1f) * 1.2f;
                        gun2.connectedBody = rb;
                        gun2.anchor = new Vector2(-0.1f, 0f);
                        gun2.breakForce = 10000f;

                        JointAngleLimits2D jal2 = new JointAngleLimits2D();
                        jal2.min = direction < 0 ? -55 : -10;
                        jal2.max = direction < 0 ? 10 : 55f;
                        gun2.limits = jal2;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3.8f, -0.9f), new Vector2(-3.05f, -1.2f), new Vector2(-2.3f, -1.2f), new Vector2(-1.55f, -1.2f), new Vector2(-0.8f, -1.2f), new Vector2(-0.05f, -1.2f), new Vector2(0.7f, -1.2f), new Vector2(1.45f, -1.2f), new Vector2(4f, -1f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 0.75f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 10f;
                            wj.breakForce = 15000f;

                            if (i == wheels - 1) 
                            { 
                                js.frequency = 5f;
                                wheel.transform.localScale *= 1.7f;
                            }
                            else
                            {
                                wheel.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "wheel.png");
                                wheel.GetComponent<PhysicalBehaviour>().Properties=ModAPI.FindPhysicalProperties("Metal");
                            }

                            wj.suspension = js;

                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        vehicle.objects[wheels] = gun.gameObject;
                        vehicle.objects[wheels+1] = gun2.gameObject;

                        //CAR BEHAVIOUR
                        vehicle.acceleration = 0.01f;
                        car.MotorSpeed = -2250;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(3f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Zumwalt Stealth Destroyer
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Zumwalt Stealth Destroyer",
                    DescriptionOverride = "Stealthy boat with guns.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Military/Destroyer/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Military/Destroyer/";
                        int floaters = 26;
                        float massMult = 35f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x > 0 ? 1f : -1f;
                        Instance.GetComponent<PhysicalBehaviour>().Properties.Buoyancy = 0.25f;
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        Instance.transform.localScale*=2f;

                        //FLOATERS
                        GameObject w = ModAPI.FindSpawnable("Plastic Barrel").Prefab;

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        vehicle.objects = new GameObject[floaters+4];

                        for (int i = 0; i < floaters; i++)
                        {
                            GameObject floater = GameObject.Instantiate(w, Instance.transform.position + new Vector3((-18f+(1.5f*i))*direction, -3.15f, 0f), Quaternion.identity);
                            floater.GetComponent<PhysicalBehaviour>().Selectable=false;
                            floater.GetComponent<PhysicalBehaviour>().Properties.SoftImpact=null;
                            floater.GetComponent<PhysicalBehaviour>().Properties.Buoyancy=massMult/2f;
                            floater.layer=10;
                            floater.transform.parent=Instance.transform;
                            GameObject.Destroy(floater.GetComponent<SpriteRenderer>());
                            HingeJoint2D hj = floater.AddComponent<HingeJoint2D>();
                            hj.connectedBody = Instance.GetComponent<Rigidbody2D>();
                            //hj.breakForce = 4000f;
                            floater.AddComponent<JointBreak>().action = () => { GameObject.Destroy(floater); };
                            vehicle.objects[i] = floater;
                        }

                        //GUN 1
                        HingeJoint2D gun = GameObject.Instantiate(ModAPI.FindSpawnable("Detached 120mm Cannon").Prefab, Instance.transform.position+new Vector3(-6f*direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun.GetComponent<Rigidbody2D>().angularDrag = 1000000f;
                        gun.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        gun.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"gun.png");
                        gun.gameObject.FixColliders();
                        gun.transform.localScale = new Vector3(-direction, 1f, 1f)*2f;
                        gun.connectedBody = rb;
                        gun.anchor = new Vector2(1f, 0f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -50 : -10;
                        jal.max = direction < 0 ? 10 : 50;
                        gun.limits = jal;
                        vehicle.objects[floaters]=gun.gameObject;

                        //GUN 2
                        HingeJoint2D gun2 = GameObject.Instantiate(ModAPI.FindSpawnable("Detached 120mm Cannon").Prefab, Instance.transform.position + new Vector3(7.5f*direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun2.GetComponent<Rigidbody2D>().angularDrag = 1000000f;
                        gun2.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        gun2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "gun.png");
                        gun2.gameObject.FixColliders();
                        gun2.transform.localScale = new Vector3(direction, 1f, 1f) * 2f;
                        gun2.connectedBody = rb;
                        gun2.anchor = new Vector2(1f, 0f);
                        JointAngleLimits2D jal2 = new JointAngleLimits2D();
                        jal2.min = direction < 0 ? 5 : -70;
                        jal2.max = direction < 0 ? 70 : -5;
                        gun2.limits = jal2;
                        vehicle.objects[floaters+1] = gun2.gameObject;

                        //GUN 3
                        HingeJoint2D gun3 = GameObject.Instantiate(ModAPI.FindSpawnable("Detached 30mm HEAT Cannon").Prefab, Instance.transform.position + new Vector3(14.25f*direction, -0.4f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        gun3.GetComponent<Rigidbody2D>().angularDrag = 1000000f;
                        gun3.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        gun3.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "smallGun.png");
                        gun3.gameObject.FixColliders();
                        gun3.transform.localScale = new Vector3(direction, 1f, 1f);
                        gun3.connectedBody = rb;
                        gun3.anchor = new Vector2(-0.25f, 0f);
                        JointAngleLimits2D jal3 = new JointAngleLimits2D();
                        jal3.min = direction < 0 ? -5 : -50;
                        jal3.max = direction < 0 ? 50 : 5;
                        gun3.limits = jal3;
                        vehicle.objects[floaters+2] = gun3.gameObject;

                        //BOAT BEHAVIOUR
                        Boat boat = Instance.gameObject.AddComponent<Boat>();
                        boat.speed=300f*direction;
                        boat.source = Instance.AddComponent<AudioSource>();
                        boat.loop = ModAPI.LoadSound(path+"loop.wav");
                        boat.start = ModAPI.LoadSound(path+"start.mp3");
                        boat.stop = ModAPI.LoadSound(path+"stop.mp3");

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-12f * direction, -3f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*2f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[floaters+3] = engine.gameObject;

                        engine.GetComponent<PhysicalBehaviour>().TrueInitialMass *= 0.0001f;
                        engine.GetComponent<PhysicalBehaviour>().InitialMass *= 0.0001f;
                        engine.GetComponent<Rigidbody2D>().mass *= 0.0001f;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            engine.GetComponent<PhysicalBehaviour>().TrueInitialMass *= 20000f;
                            engine.GetComponent<PhysicalBehaviour>().InitialMass *= 20000f;
                            engine.GetComponent<Rigidbody2D>().mass *= 20000f;
                            
                            GameObject.Destroy(boat, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #endregion

            #region Utility
            #region Combine Harvester
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Combine Harvester",
                    DescriptionOverride = "Harvests combines.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Utility/CombineHarvester/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Utility/CombineHarvester/";
                        int wheels = 6;
                        float massMult = 10f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //HARVESTER
                        HingeJoint2D harvester = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position+new Vector3(8.8f*direction, -1.2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        harvester.transform.localScale=new Vector3(direction,1f,1f);
                        harvester.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"hCol.png");
                        harvester.gameObject.FixColliders();
                        harvester.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "harvester.png");
                        harvester.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        harvester.GetComponent<SpriteRenderer>().sortingOrder=1;
                        harvester.connectedBody=rb;
                        harvester.anchor=new Vector2(-2.5f, 0.5f);
                        harvester.breakForce=30000f;

                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? 5 : -15f;
                        jal.max = direction < 0 ? 15 : -5f;
                        harvester.limits = jal;
                        harvester.limits=jal;

                        AudioSource harvesterAs = harvester.gameObject.AddComponent<AudioSource>();
                        harvesterAs.clip = ModAPI.LoadSound(path + "harvester.wav");
                        harvesterAs.loop=true;
                        harvesterAs.Play();

                        //BLADE
                        WheelJoint2D blade = GameObject.Instantiate(ModAPI.FindSpawnable("Wheel").Prefab, harvester.transform.position + new Vector3(0.9f*direction, 0.5f, 0f), Quaternion.identity).AddComponent<WheelJoint2D>();
                        blade.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                        blade.gameObject.layer=10;
                        blade.transform.localScale*=1.5f;
                        blade.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"blade.png");
                        blade.gameObject.FixColliders();
                        blade.GetComponent<SpriteRenderer>().sortingOrder=0;
                        blade.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        blade.connectedBody=harvester.GetComponent<Rigidbody2D>();
                        blade.connectedAnchor=new Vector2(0.9f, 0.5f);
                        JointSuspension2D zero = blade.suspension;
                        zero.dampingRatio = 0f;
                        zero.frequency = 100000f;
                        JointMotor2D bladeMotor = new JointMotor2D();
                        bladeMotor.motorSpeed = -750 * direction;
                        bladeMotor.maxMotorTorque = 750;
                        blade.motor = bladeMotor;
                        blade.useMotor = true;
                        blade.suspension=zero;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-1.4f, -1.75f), new Vector2(3.8f, -1.4f), new Vector2(2.5f, -2.2f), new Vector2(4.6f, -2.2f), new Vector2(3.3f, -2.4f), new Vector2(3.85f, -2.4f), };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 7];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                            wheel.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"/wheel.png");
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;

                            if(i==0)
                            {
                                wheel.transform.localScale *= 1.8f;
                                wheel.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "/wheel2.png");
                                wheel.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Rubber");
                            }
                            else if(i>3)
                            {
                                wheel.transform.localScale *= 0.5f;
                            }

                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //GRINDERS
                        GameObject grinder1 = GameObject.Instantiate(w, harvester.transform.position + new Vector3(1.75f * direction, 1.1f, 0f), Quaternion.identity);
                        grinder1.transform.localScale *= 1.2f;
                        grinder1.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinderJoint = grinder1.AddComponent<WheelJoint2D>();
                        grinderJoint.connectedBody = harvester.GetComponent<Rigidbody2D>();
                        grinderJoint.connectedAnchor = new Vector3(1.75f, 1.1f, 0f);
                        grinderJoint.suspension = zero;
                        JointMotor2D grinderMotor = new JointMotor2D();
                        grinderMotor.motorSpeed = 1500f * direction;
                        grinderMotor.maxMotorTorque = 1500000f;
                        grinderJoint.motor = grinderMotor;
                        grinderJoint.useMotor = true;
                        grinder1.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                        grinder1.transform.parent=blade.transform;
                        GameObject.Destroy(grinder1.GetComponent<SpriteRenderer>());

                        GameObject grinder2 = GameObject.Instantiate(w, harvester.transform.position + new Vector3(1.75f * direction, -0.3f, 0f), Quaternion.identity);
                        grinder2.transform.localScale *= 1.2f;
                        grinder2.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinder2Joint = grinder2.AddComponent<WheelJoint2D>();
                        grinder2Joint.connectedBody = harvester.GetComponent<Rigidbody2D>();
                        grinder2Joint.connectedAnchor = new Vector3(1.75f, -0.3f, 0f);
                        grinder2Joint.suspension = zero;
                        JointMotor2D grinder2Motor = new JointMotor2D();
                        grinder2Motor.motorSpeed = -1500f * direction;
                        grinder2Motor.maxMotorTorque = 15000000f;
                        grinder2Joint.motor = grinder2Motor;
                        grinder2Joint.useMotor = true;
                        grinder2.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                        grinder2.transform.parent = blade.transform;
                        GameObject.Destroy(grinder2.GetComponent<SpriteRenderer>());

                        GameObject grinder3 = GameObject.Instantiate(w, harvester.transform.position + new Vector3(0.25f * direction, 1.1f, 0f), Quaternion.identity);
                        grinder3.transform.localScale *= 1.2f;
                        grinder3.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinder3Joint = grinder3.AddComponent<WheelJoint2D>();
                        grinder3Joint.connectedBody = harvester.GetComponent<Rigidbody2D>();
                        grinder3Joint.connectedAnchor = new Vector3(0.25f, 1.1f, 0f);
                        grinder3Joint.suspension = zero;
                        grinder3Joint.motor = grinder2Motor;
                        grinder3Joint.useMotor = true;
                        grinder3.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                        grinder3.transform.parent = blade.transform;
                        GameObject.Destroy(grinder3.GetComponent<SpriteRenderer>());

                        GameObject grinder4 = GameObject.Instantiate(w, harvester.transform.position + new Vector3(0.25f * direction, -0.3f, 0f), Quaternion.identity);
                        grinder4.transform.localScale *= 1.2f;
                        grinder4.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinder4Joint = grinder4.AddComponent<WheelJoint2D>();
                        grinder4Joint.connectedBody = harvester.GetComponent<Rigidbody2D>();
                        grinder4Joint.connectedAnchor = new Vector3(0.25f, -0.3f, 0f);
                        grinder4Joint.suspension = zero;
                        grinder4Joint.motor = grinder2Motor;
                        grinder4Joint.useMotor = true;
                        grinder4.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                        grinder4.transform.parent = blade.transform;
                        GameObject.Destroy(grinder4.GetComponent<SpriteRenderer>());

                        vehicle.objects[wheels] = harvester.gameObject;
                        vehicle.objects[wheels+1] = grinder1.gameObject;
                        vehicle.objects[wheels+2] = grinder2.gameObject;
                        vehicle.objects[wheels+3] = grinder3.gameObject;
                        vehicle.objects[wheels+4] = grinder4.gameObject;
                        vehicle.objects[wheels+5] = blade.gameObject;

                        //CAR BEHAVIOUR
                        vehicle.acceleration=0.03f;
                        vehicle.maxTorque=300f;
                        car.MotorSpeed = -750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        harvester.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            GameObject.Destroy(harvesterAs);
                            bladeMotor.motorSpeed = 0f;
                            blade.motor = bladeMotor;
                            grinderMotor.motorSpeed = 0f;
                            grinderJoint.motor = grinderMotor;
                            grinder2Joint.motor = grinderMotor;
                            grinder3Joint.motor = grinderMotor;
                            grinder4Joint.motor = grinderMotor;
                        };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-1f * direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.75f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels+6] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);
                            GameObject.Destroy(harvesterAs);
                            bladeMotor.motorSpeed = 0f;
                            blade.motor = bladeMotor;
                            grinderMotor.motorSpeed = 0f;
                            grinderJoint.motor = grinderMotor;
                            grinder2Joint.motor = grinderMotor;
                            grinder3Joint.motor = grinderMotor;
                            grinder4Joint.motor = grinderMotor;

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Fork Lift
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Fork Lift",
                    DescriptionOverride = "Lifts forks.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Utility/ForkLift/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Utility/ForkLift/";
                        int wheels = 2;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");

                        //FORK
                        SliderJoint2D fork = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2f * direction, -0.5f, 0f), Quaternion.identity).AddComponent<SliderJoint2D>();
                        fork.transform.localScale=new Vector3(direction, 1f, 1f);
                        fork.GetComponent<PhysicalBehaviour>().TrueInitialMass = 2f;
                        fork.GetComponent<PhysicalBehaviour>().InitialMass = 2f;
                        fork.GetComponent<Rigidbody2D>().mass = 2f;
                        fork.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"fCol.png");
                        fork.gameObject.FixColliders();
                        fork.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "fork.png");
                        fork.connectedBody=rb;
                        fork.connectedAnchor = new Vector2(2f, -0.5f);
                        fork.autoConfigureAngle=false;
                        fork.angle=270;
                        JointTranslationLimits2D jtl = new JointTranslationLimits2D();
                        jtl.max=2f;
                        jtl.min=0f;
                        fork.limits=jtl;
                        JointMotor2D forkMotor = new JointMotor2D();
                        forkMotor.maxMotorTorque=1000f;
                        forkMotor.motorSpeed=-0.5f;
                        fork.motor=forkMotor;
                        fork.useMotor=true;
                        fork.gameObject.AddComponent<ForkLift>();

                        //WEIGHT
                        HingeJoint2D weight = GameObject.Instantiate(ModAPI.FindSpawnable("1000kg Weight").Prefab, Instance.transform.position + new Vector3(-1.4f * direction, -0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        weight.GetComponent<PhysicalBehaviour>().Selectable=false;
                        weight.connectedBody=rb;
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = 0f;
                        jal.max = 0f;
                        weight.limits = jal;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.4f, -1f), new Vector2(0.6f, -1f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 4];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.1f;
                            wheel.GetComponent<Rigidbody2D>().mass = 250f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;


                            foreach (Collider2D c in fork.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(wheel.GetComponent<Collider2D>(), c, true);
                            }
                        }

                        vehicle.objects[wheels]=fork.gameObject;
                        vehicle.objects[wheels+1]=weight.gameObject;
                        
                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(0f, 0.35f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels + 2] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-1.9f * direction, -0.15f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 3] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Tractor
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Tractor",
                    DescriptionOverride = "Its big and green.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Utility/Tractor/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Utility/Tractor/";
                        int wheels = 2;
                        float massMult = 5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.3f, -1f), new Vector2(2.4f, -1.25f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 1];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"wheel.png");
                            if(i==0){wheel.transform.localScale *= 3.5f;}
                            else { wheel.transform.localScale *= 3f; }
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.5f;
                            js.frequency = 15f;
                            wj.suspension = js;
                            wj.breakForce = 35000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -1500f;
                        vehicle.acceleration=0.25f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(1.5f * direction, 0.25f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.5f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Crawler Crane
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Crawler Crane",
                    DescriptionOverride = "Use the crane to pick stuff up. (This can be buggy.)",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Utility/CrawlerCrane/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Utility/CrawlerCrane/";
                        int wheels = 6;
                        float massMult = 12f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        background.sortingOrder=1;
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = 
                        { 
                            new Vector2(-6.75f, -4.45f),
                            new Vector2(-5.25f, -4.35f),
                            new Vector2(-3.5f, -4.35f),
                            new Vector2(-1.75f, -4.35f),
                            new Vector2(0f, -4.35f),
                            new Vector2(1.3f, -4.45f)
                        };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels+21];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "wheel.png");
                            wheel.GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Metal");
                            wheel.transform.localScale = Vector3.one*1.5f;
                            if(i==0 || i==wheels-1){ wheel.transform.localScale = Vector3.one*1.1f; }
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.5f;
                            js.frequency = 10f;

                            wj.suspension = js;

                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CRANE
                        HingeJoint2D crane = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(0f, -0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        crane.transform.localScale = new Vector3(direction, 1f, 1f);
                        crane.GetComponent<Rigidbody2D>().angularDrag = 900000000f;
                        crane.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        crane.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "crane.png");
                        crane.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        crane.gameObject.FixColliders();
                        crane.connectedBody = rb;
                        crane.anchor = new Vector2(-4f, -1f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -20 : -45f;
                        jal.max = direction < 0 ? 45 : 20f;
                        crane.limits = jal;
                        ToggleJointMotor tjm = crane.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = 4f * direction;
                        tjm.maxTorque = 10000f;
                        crane.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        crane.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        crane.GetComponent<Rigidbody2D>().mass *= massMult;
                        vehicle.objects[wheels]=crane.gameObject;

                        float wireMass = 1f;

                        //WIRE
                        HingeJoint2D wireBody = GameObject.Instantiate(ModAPI.FindSpawnable("Plastic Barrel").Prefab, crane.transform.position + new Vector3(8.5f * direction, 3.4f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        wireBody.gameObject.layer=13;
                        wireBody.transform.localScale = new Vector3(direction, 1f, 1f);
                        wireBody.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path+"wireBody.png");
                        wireBody.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        GameObject.Destroy(wireBody.GetComponent<BoxCollider2D>());
                        wireBody.GetComponent<PhysicalBehaviour>().TrueInitialMass = wireMass;
                        wireBody.GetComponent<PhysicalBehaviour>().InitialMass = wireMass;
                        wireBody.GetComponent<Rigidbody2D>().mass = wireMass;
                        wireBody.connectedBody = crane.GetComponent<Rigidbody2D>();
                        wireBody.anchor = new Vector2(0f, 0.35f);
                        vehicle.objects[wheels+1] = wireBody.gameObject;

                        HingeJoint2D lastBody = wireBody;

                        for (int i = 2; i < 18; i++)
                        {
                            HingeJoint2D currentBody = GameObject.Instantiate(ModAPI.FindSpawnable("Plastic Barrel").Prefab, Instance.transform.position + new Vector3(8.5f * direction, 3.6f - (0.65f * i), 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                            currentBody.gameObject.layer = 13;
                            currentBody.transform.localScale = new Vector3(direction, 1f, 1f);
                            currentBody.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path+"wireBody.png");
                            currentBody.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            GameObject.Destroy(currentBody.GetComponent<BoxCollider2D>());
                            currentBody.GetComponent<PhysicalBehaviour>().TrueInitialMass = wireMass;
                            currentBody.GetComponent<PhysicalBehaviour>().InitialMass = wireMass;
                            currentBody.GetComponent<Rigidbody2D>().mass = wireMass;
                            currentBody.connectedBody = lastBody.GetComponent<Rigidbody2D>();
                            currentBody.anchor = new Vector2(0f, 0.35f);

                            foreach (Collider2D c in crane.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(currentBody.GetComponent<BoxCollider2D>(), c, true);
                            }

                            vehicle.objects[wheels+i] = currentBody.gameObject;
                            lastBody = currentBody;
                        }

                        //HOOK
                        HingeJoint2D hook = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(8.5f * direction, -8.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        hook.transform.localScale = new Vector3(direction, 1f, 1f)*0.75f;
                        hook.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "hook.png");
                        hook.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        hook.gameObject.FixColliders();
                        hook.GetComponent<PhysicalBehaviour>().TrueInitialMass = 5f;
                        hook.GetComponent<PhysicalBehaviour>().InitialMass = 5f;
                        hook.GetComponent<Rigidbody2D>().mass = 5f;
                        hook.connectedBody = lastBody.GetComponent<Rigidbody2D>();
                        hook.anchor = new Vector2(0f, 0.4f);
                        hook.gameObject.AddComponent<Hook>();
                        vehicle.objects[wheels + 19] = hook.gameObject;
                        foreach (Collider2D c in crane.GetComponents<Collider2D>())
                        {
                            foreach (Collider2D c2 in hook.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(c, c2, true);
                            }
                        }
                        //CAR BEHAVIOUR
                        vehicle.acceleration = 0.02f;
                        vehicle.maxTorque = 150f;
                        car.MotorSpeed = -1250;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-3f * direction, -2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels+20] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region School Bus
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "School Bus",
                    DescriptionOverride = "Goes to school and back.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Utility/Bus/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Utility/Bus/";
                        int wheels = 2;
                        float massMult = 4f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.9f, -1.4f), new Vector2(6.4f, -1.4f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.6f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -3200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(4.3f, 0.22f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = -direction;
                        door2.anchor = new Vector2(5.3f, 0.22f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels+1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(6.25f * direction, -0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.2f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels+2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #endregion

            #region Trucks
            #region Peterbilt 379
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Peterbilt 379",
                    DescriptionOverride = "Square truck.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/Peterbilt379/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/Peterbilt379/";
                        int wheels = 3;
                        float massMult = 5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //ATTACH POINT
                        GameObject attachPoint = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-4.35f*direction, -1.4f, 0f), Quaternion.identity);
                        attachPoint.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
                        attachPoint.transform.parent = Instance.transform;
                        attachPoint.transform.localScale = new Vector3(8f, 2f, 0f);
                        attachPoint.FixColliders();
                        foreach (Collider2D c in attachPoint.GetComponents<Collider2D>()) { c.isTrigger = true; }
                        attachPoint.GetComponent<SpriteRenderer>().sprite=null;
                        attachPoint.GetComponent<Rigidbody2D>().isKinematic = true;
                        attachPoint.AddComponent<AttachPoint>();//this doesn't do anything, but you cant set a new tag in code so this works instead

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-6f, -2.5f), new Vector2(-3.9f, -2.5f), new Vector2(6.2f, -2.5f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 2f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -3500f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(3.5f, -0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(5.5f * direction, -0.75f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.5f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Iveco S-Way
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Iveco S-Way",
                    DescriptionOverride = "Red truck.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/IvecoSWay/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/IvecoSWay/";
                        int wheels = 2;
                        float massMult = 4f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //ATTACH POINT
                        GameObject attachPoint = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-1.9f * direction, -1.45f, 0f), Quaternion.identity);
                        attachPoint.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
                        attachPoint.transform.parent = Instance.transform;
                        attachPoint.transform.localScale = new Vector3(5f, 2f, 0f);
                        attachPoint.FixColliders();
                        foreach (Collider2D c in attachPoint.GetComponents<Collider2D>()) { c.isTrigger = true; }
                        attachPoint.GetComponent<SpriteRenderer>().sprite = null;
                        attachPoint.GetComponent<Rigidbody2D>().isKinematic = true;
                        attachPoint.AddComponent<AttachPoint>();//this doesn't do anything, but you cant set a new tag in code so this works instead

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3.1f, -2.5f), new Vector2(2.3f, -2.5f)};

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.8f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -3250f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(4f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position+new Vector3(1.5f*direction, -0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.25f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels+1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () => 
                        { 
                            car.Activated=false; 
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position+new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic=true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Kenworth T680
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Kenworth T680",
                    DescriptionOverride = "Truck.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/KenworthT680/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/KenworthT680/";
                        int wheels = 3;
                        float massMult = 4.5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //ATTACH POINT
                        GameObject attachPoint = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-3.5f * direction, -1.8f, 0f), Quaternion.identity);
                        attachPoint.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
                        attachPoint.transform.parent = Instance.transform;
                        attachPoint.transform.localScale = new Vector3(7f, 2f, 0f);
                        attachPoint.FixColliders();
                        foreach (Collider2D c in attachPoint.GetComponents<Collider2D>()) { c.isTrigger = true; }
                        attachPoint.GetComponent<SpriteRenderer>().sprite = null;
                        attachPoint.GetComponent<Rigidbody2D>().isKinematic = true;
                        attachPoint.AddComponent<AttachPoint>();//this doesn't do anything, but you cant set a new tag in code so this works instead

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-5.55f, -2.8f), new Vector2(-3.35f, -2.8f), new Vector2(5.9f, -2.8f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 2f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -3400f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(4.5f, -0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(5.5f * direction, -1.2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.5f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Box Truck
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "International Box Truck",
                    DescriptionOverride = "Boxed truck.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/BoxTruck/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/BoxTruck/";
                        int wheels = 3;
                        float massMult = 4f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3.25f, -1.5f), new Vector2(-1.5f, -1.5f), new Vector2(4.75f, -1.5f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.5f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //BACK DOOR
                        HingeJoint2D backDoor = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-5.95f * direction, 0.715f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        backDoor.transform.localScale = new Vector3(direction, 1f, 1f);
                        backDoor.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        backDoor.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        backDoor.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "backDoor.png");
                        backDoor.gameObject.FixColliders();
                        backDoor.connectedBody = rb;
                        backDoor.anchor = new Vector2(0.075f, 1.55f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -90f : 0f;
                        jal.max = direction < 0 ? 0f : 90f;
                        backDoor.limits = jal;
                        ToggleJointMotor tjm = backDoor.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = -15f * direction;
                        tjm.maxTorque = 1000f;
                        vehicle.objects[wheels] = backDoor.gameObject;

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -3000f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(4f, -0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels+1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(4.75f * direction, -0.4f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Dump Truck
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "International Dump Truck",
                    DescriptionOverride = "Dump truck.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/DumpTruck/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/DumpTruck/";
                        int wheels = 2;
                        float massMult = 3f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //DUMP
                        HingeJoint2D dump = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-0.3f*direction, -0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        dump.transform.localScale = new Vector3(direction, 1f, 1f);
                        dump.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        dump.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        dump.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "dumpCol.png");
                        dump.gameObject.FixColliders();
                        dump.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "dump.png");
                        dump.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        dump.connectedBody = rb;
                        dump.anchor = new Vector2(-4f, -0.25f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? 0f : -50f;
                        jal.max = direction < 0 ? 50 : 0f;
                        dump.limits = jal;
                        ToggleJointMotor tjm = dump.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = 10f * direction;
                        tjm.maxTorque = 500f;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3.25f, -1.25f), new Vector2(3.7f, -1.25f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.5f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;

                            foreach(Collider2D c in dump.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(c, wheel.GetComponent<Collider2D>(), true);
                            }
                        }

                        vehicle.objects[wheels] = dump.gameObject;

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -3000f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction=direction;
                        door.anchor = new Vector2(3f, -0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(3.8f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Garbage Truck
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Garbage Truck",
                    DescriptionOverride = "A truck with garbage.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/GarbageTruck/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/GarbageTruck/";
                        int wheels = 3;
                        float massMult = 4f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //BACK END
                        HingeJoint2D back = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-4.7f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        back.transform.localScale = new Vector3(direction, 1f, 1f);
                        back.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "backCol.png");
                        back.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        back.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        back.gameObject.FixColliders();
                        back.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "back.png");
                        back.connectedBody = rb;
                        back.anchor = new Vector2(1.5f, 2.2f);
                        back.breakForce = 25000f;
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -60 : 0;
                        jal.max = direction < 0 ? 0 : 60;
                        back.limits = jal;
                        ToggleJointMotor tjm = back.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = -10f * direction;
                        tjm.maxTorque = 500f;

                        //GRINDERS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        GameObject grinder1 = GameObject.Instantiate(w, back.transform.position + new Vector3(-0.7f * direction, -0.7f, 0f), Quaternion.identity);
                        grinder1.transform.localScale *= 1.6f;
                        grinder1.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinderJoint = grinder1.AddComponent<WheelJoint2D>();
                        grinderJoint.connectedBody = back.GetComponent<Rigidbody2D>();
                        grinderJoint.connectedAnchor = new Vector3(-0.7f, -0.7f, 0f);
                        JointSuspension2D grinderJs = grinderJoint.suspension;
                        grinderJs.dampingRatio = 0f;
                        grinderJs.frequency = 1000000f;
                        grinderJoint.suspension = grinderJs;
                        JointMotor2D grinderMotor = new JointMotor2D();
                        grinderMotor.motorSpeed = 250 * direction;
                        grinderMotor.maxMotorTorque = 250;
                        grinderJoint.motor = grinderMotor;
                        grinderJoint.useMotor = true;
                        grinder1.GetComponent<SpriteRenderer>().sprite=null;

                        GameObject grinder2 = GameObject.Instantiate(w, back.transform.position + new Vector3(-0.16f * direction, 0.8f, 0f), Quaternion.identity);
                        grinder2.transform.localScale *= 1.4f;
                        grinder2.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinder2Joint = grinder2.AddComponent<WheelJoint2D>();
                        grinder2Joint.connectedBody = back.GetComponent<Rigidbody2D>();
                        grinder2Joint.connectedAnchor = new Vector3(-0.16f, 0.8f, 0f);
                        JointSuspension2D grinder2Js = grinder2Joint.suspension;
                        grinder2Js.dampingRatio = 0f;
                        grinder2Js.frequency = 1000000f;
                        grinder2Joint.suspension = grinder2Js;
                        JointMotor2D grinder2Motor = new JointMotor2D();
                        grinder2Motor.motorSpeed = -250 * direction;
                        grinder2Motor.maxMotorTorque = 250;
                        grinder2Joint.motor = grinder2Motor;
                        grinder2Joint.useMotor = true;
                        grinder2.GetComponent<SpriteRenderer>().sprite = null;

                        foreach (Collider2D c in Instance.GetComponents<Collider2D>())
                        {
                            Physics2D.IgnoreCollision(grinder1.GetComponent<Collider2D>(), c, true);
                            Physics2D.IgnoreCollision(grinder2.GetComponent<Collider2D>(), c, true);
                        }

                        //WHEELS
                        Vector2[] wps = { new Vector2(-3.57f, -1.9f), new Vector2(-1.7f, -1.9f), new Vector2(4.55f, -1.9f) };

                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        vehicle.objects = new GameObject[wheels+3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.75f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder=2;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;

                            foreach (Collider2D c in back.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(wheel.GetComponent<Collider2D>(), c, true);
                            }
                        }

                        vehicle.objects[wheels] = back.gameObject;
                        vehicle.objects[wheels+1] = grinder1;
                        vehicle.objects[wheels+2] = grinder2;

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -2300f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(6f, -0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(3.8f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion

            #region Basic Trailer
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Basic Trailer",
                    DescriptionOverride = "Basic Semi Trailer.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/Trailer/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/Trailer/";
                        int wheels = 2;
                        float massMult = 2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-7.25f, -2.25f), new Vector2(-5f, -2.25f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        vehicle.objects = new GameObject[wheels+1];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 2f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            vehicle.objects[i] = wheel;
                        }

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-9.55f * direction, 0.82225f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        door.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"door.png");
                        door.gameObject.FixColliders();
                        door.connectedBody=rb;
                        door.anchor = new Vector2(0.05f, 2.25f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -90f : 0f;
                        jal.max = direction < 0 ? 0f : 90f;
                        door.limits = jal;
                        ToggleJointMotor tjm = door.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed=-15f*direction;
                        tjm.maxTorque=1000f;
                        vehicle.objects[wheels]=door.gameObject;

                        //CONNECTOR
                        GameObject connector = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(7.2f*direction, -1.05f, 0f), Quaternion.identity);
                        connector.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite("locator.png");
                        connector.transform.parent = Instance.transform;
                        connector.transform.localScale = new Vector3(6.5f, 1f, 0f);
                        connector.FixColliders();
                        connector.GetComponent<SpriteRenderer>().sprite=null;
                        foreach (Collider2D c in connector.GetComponents<Collider2D>()){c.isTrigger=true;}
                        connector.GetComponent<Rigidbody2D>().isKinematic=true;
                        connector.AddComponent<Connector>();
                    }
                }
            );
            #endregion
            #region Oil Trailer
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Oil Trailer",
                    DescriptionOverride = "Contains flammable liquid.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/OilTrailer/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/OilTrailer/";
                        int wheels = 2;
                        float massMult = 2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-7.6f, -1.5f), new Vector2(-5.45f, -1.5f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        vehicle.objects = new GameObject[wheels + 1];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 2f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            vehicle.objects[i] = wheel;
                        }

                        //TANK
                        HingeJoint2D tank = GameObject.Instantiate(ModAPI.FindSpawnable("Red Barrel").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        tank.transform.localScale=new Vector3(direction, 1f, 1f);
                        tank.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"tankCol.png");
                        tank.gameObject.FixColliders();
                        tank.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "tank.png");
                        tank.connectedBody=rb;
                        JointAngleLimits2D zero = new JointAngleLimits2D();
                        zero.min = 0f;
                        zero.max = 0f;
                        tank.limits=zero;
                        tank.breakForce=20000f;
                        tank.GetComponent<ExplosiveBehaviour>().OnExplode.AddListener( () => 
                        {
                            ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters
                            {
                                Position = Instance.transform.position+new Vector3(-5f, 3.5f),
                                CreateParticlesAndSound = true,
                                LargeExplosionParticles = true,
                                DismemberChance = 0.75f,
                                FragmentForce = 50,
                                FragmentationRayCount = 32,
                                Range = 3000
                            });

                            ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters
                            {
                                Position = Instance.transform.position + new Vector3(5f, 3.5f),
                                CreateParticlesAndSound = true,
                                LargeExplosionParticles = true,
                                DismemberChance = 0.75f,
                                FragmentForce = 50,
                                FragmentationRayCount = 32,
                                Range = 3000
                            });
                        });

                        //CONNECTOR
                        GameObject connector = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(7.25f * direction, -0.35f, 0f), Quaternion.identity);
                        connector.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
                        connector.transform.parent = Instance.transform;
                        connector.transform.localScale = new Vector3(5f, 1f, 0f);
                        connector.FixColliders();
                        connector.GetComponent<SpriteRenderer>().sprite = null;
                        foreach (Collider2D c in connector.GetComponents<Collider2D>()) { c.isTrigger = true; }
                        connector.GetComponent<Rigidbody2D>().isKinematic = true;
                        connector.AddComponent<Connector>();
                    }
                }
            );
            #endregion
            #region Flatbed Trailer
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Flatbed Trailer",
                    DescriptionOverride = "Flat trailer with a ramp.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Trucks/FlatbedTrailer/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Trucks/FlatbedTrailer/";
                        int wheels = 2;
                        float massMult = 1.25f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-5.5f, -1.5f), new Vector2(-2.5f, -1.5f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        vehicle.objects = new GameObject[wheels + 1];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 2f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            vehicle.objects[i] = wheel;
                        }

                        //RAMP
                        HingeJoint2D ramp = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-6.3f * direction, 2.25f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        ramp.transform.localScale = new Vector3(direction, 1f, 1f);
                        ramp.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        ramp.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        ramp.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "ramp.png");
                        ramp.gameObject.FixColliders();
                        ramp.connectedBody = rb;
                        ramp.anchor = new Vector2(-0.1f, -2.7f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -10f : -112f;
                        jal.max = direction < 0 ? 112f : 10f;
                        ramp.limits = jal;
                        ToggleJointMotor tjm = ramp.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = 15f*direction;
                        tjm.maxTorque = 1000f;
                        vehicle.objects[wheels] = ramp.gameObject;

                        //CONNECTOR
                        GameObject connector = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(7.25f * direction, -0.4f, 0f), Quaternion.identity);
                        connector.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
                        connector.transform.parent = Instance.transform;
                        connector.transform.localScale = new Vector3(5f, 1f, 0f);
                        connector.FixColliders();
                        connector.GetComponent<SpriteRenderer>().sprite = null;
                        foreach (Collider2D c in connector.GetComponents<Collider2D>()) { c.isTrigger = true; }
                        connector.GetComponent<Rigidbody2D>().isKinematic = true;
                        connector.AddComponent<Connector>();
                    }
                }
            );
            #endregion
            #endregion

            #region Emergency
            #region Police Car
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Police Car",
                    DescriptionOverride = "Police.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Emergency/PoliceCar/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Emergency/PoliceCar/";
                        int wheels = 2;
                        float massMult = 1.2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //LIGHTS
                        LightSprite red = ModAPI.CreateLight(Instance.transform, Color.red, 4, 0);
                        red.transform.localPosition=new Vector3(0f, 1.25f, 0f);
                        LightSprite blue = ModAPI.CreateLight(Instance.transform, Color.blue, 4, 0);
                        blue.transform.localPosition = new Vector3(-0.75f, 1.25f, 0f);
                        PoliceLights pl = Instance.AddComponent<PoliceLights>();
                        pl.l1=red;
                        pl.l2=blue;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.1f, -0.6f), new Vector2(2.5f, -0.6f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.2f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        vehicle.acceleration=0.1f;
                        car.MotorSpeed = -3200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-0.25f, -0.2f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(1.1f, -0.2f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.3f * direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Swat Car
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Swat Car",
                    DescriptionOverride = "Swat.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Emergency/SwatCar/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Emergency/SwatCar/";
                        int wheels = 2;
                        float massMult = 2.5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //LIGHTS
                        LightSprite red = ModAPI.CreateLight(Instance.transform, Color.red, 4, 0);
                        red.transform.localPosition = new Vector3(1f, 1.25f, 0f);
                        LightSprite blue = ModAPI.CreateLight(Instance.transform, Color.blue, 4, 0);
                        blue.transform.localPosition = new Vector3(0.25f, 1.25f, 0f);
                        PoliceLights pl = Instance.AddComponent<PoliceLights>();
                        pl.l1 = red;
                        pl.l2 = blue;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-1.8f, -0.75f), new Vector2(2.7f, -0.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.2f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //BACK DOOR
                        HingeJoint2D backDoor = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        backDoor.transform.localScale = new Vector3(direction, 1f, 1f);
                        backDoor.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        backDoor.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        backDoor.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "backDoor.png");
                        backDoor.gameObject.FixColliders();
                        backDoor.connectedBody = rb;
                        backDoor.anchor = new Vector2(-3.7f, 1.55f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -90f : 0f;
                        jal.max = direction < 0 ? 0f : 90f;
                        backDoor.limits = jal;
                        ToggleJointMotor tjm = backDoor.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = -15f * direction;
                        tjm.maxTorque = 1000f;
                        vehicle.objects[wheels] = backDoor.gameObject;

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -3200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(1.5f, 0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels+1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.6f * direction, 0.15f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Police Van
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Police Van",
                    DescriptionOverride = "Police van.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Emergency/PoliceVan/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Emergency/PoliceVan/";
                        int wheels = 2;
                        float massMult = 2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //LIGHTS
                        LightSprite red = ModAPI.CreateLight(Instance.transform, Color.red, 4, 0);
                        red.transform.localPosition = new Vector3(2f, 1.5f, 0f);
                        LightSprite blue = ModAPI.CreateLight(Instance.transform, Color.blue, 4, 0);
                        blue.transform.localPosition = new Vector3(1.25f, 1.5f, 0f);
                        PoliceLights pl = Instance.AddComponent<PoliceLights>();
                        pl.l1 = red;
                        pl.l2 = blue;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.1f, -0.75f), new Vector2(3f, -0.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 4];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.2f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        vehicle.acceleration = 0.05f;
                        car.MotorSpeed = -2700f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        SliderJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<SliderJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        door.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.autoConfigureAngle = false;
                        door.angle = direction < 0 ? 180f : 0f;
                        door.gameObject.AddComponent<SlideDoor>();
                        door.anchor = new Vector2(0f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(2.6f, 0.5f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //BACK DOOR
                        HingeJoint2D backDoor = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        backDoor.transform.localScale = new Vector3(direction, 1f, 1f);
                        backDoor.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        backDoor.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        backDoor.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "backDoor.png");
                        backDoor.gameObject.FixColliders();
                        backDoor.connectedBody = rb;
                        backDoor.anchor = new Vector2(-3.5f, 1.25f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -90f : 0f;
                        jal.max = direction < 0 ? 0f : 90f;
                        backDoor.limits = jal;
                        ToggleJointMotor tjm = backDoor.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = -25f * direction;
                        tjm.maxTorque = 1000f;
                        vehicle.objects[wheels+2] = backDoor.gameObject;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.75f * direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 3] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Ambulance
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Ambulance",
                    DescriptionOverride = "Save people.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Emergency/Ambulance/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Emergency/Ambulance/";
                        int wheels = 2;
                        float massMult = 2.2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //LIGHTS
                        LightSprite red = ModAPI.CreateLight(Instance.transform, Color.red, 4, 0);
                        red.transform.localPosition = new Vector3(0.9f, 1.25f, 0f);
                        LightSprite blue = ModAPI.CreateLight(Instance.transform, Color.blue, 4, 0);
                        blue.transform.localPosition = new Vector3(-4f, 1.25f, 0f);
                        PoliceLights pl = Instance.AddComponent<PoliceLights>();
                        pl.l1 = red;
                        pl.l2 = blue;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.3f, -1.2f), new Vector2(3.6f, -1.2f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.35f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //BACK DOOR
                        HingeJoint2D backDoor = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        backDoor.transform.localScale = new Vector3(direction, 1f, 1f);
                        backDoor.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        backDoor.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        backDoor.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "backDoor.png");
                        backDoor.gameObject.FixColliders();
                        backDoor.connectedBody = rb;
                        backDoor.anchor = new Vector2(-4.5f, 1.7f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -90f : 0f;
                        jal.max = direction < 0 ? 0f : 90f;
                        backDoor.limits = jal;
                        ToggleJointMotor tjm = backDoor.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = -15f * direction;
                        tjm.maxTorque = 1000f;
                        vehicle.objects[wheels] = backDoor.gameObject;

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -3200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(2.7f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels+1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(3.55f * direction, -0.2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Firetruck
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Firetruck",
                    DescriptionOverride = "A truck with fire.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Emergency/Firetruck/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Emergency/Firetruck/";
                        int wheels = 2;
                        float massMult = 4f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.9f, -1.2f), new Vector2(3.7f, -1.2f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 20];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.5f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //HOSE
                        HingeJoint2D hoseBody = GameObject.Instantiate(ModAPI.FindSpawnable("Plastic Barrel").Prefab, Instance.transform.position + new Vector3(0.35f * direction, -0.6f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        hoseBody.transform.localScale = new Vector3(direction, 1f, 1f);
                        hoseBody.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path+"hoseBody.png");
                        hoseBody.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        hoseBody.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.1f);
                        hoseBody.connectedBody = rb;
                        hoseBody.anchor = new Vector2(-0.25f, 0f);
                        vehicle.objects[wheels] = hoseBody.gameObject;

                        HingeJoint2D lastBody = hoseBody;

                        for (int i = 1; i < 14; i++)
                        {
                            HingeJoint2D currentBody = GameObject.Instantiate(ModAPI.FindSpawnable("Plastic Barrel").Prefab, Instance.transform.position + new Vector3((0.35f + (0.6f * i)) * direction, -0.6f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                            currentBody.gameObject.layer = 13;//colliding debris layer, using this as a means of preventing the extinguisher particles from colliding with this
                            currentBody.breakForce = 10000f;
                            currentBody.transform.localScale = new Vector3(direction, 1f, 1f);
                            currentBody.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path+"hoseBody.png");
                            currentBody.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            currentBody.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.1f);
                            currentBody.connectedBody = lastBody.GetComponent<Rigidbody2D>();
                            currentBody.anchor = new Vector2(-0.25f, 0f);

                            foreach (Collider2D c in Instance.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(currentBody.GetComponent<BoxCollider2D>(), c, true);
                            }
                            for (int j = 0; j < i+wheels; j++)
                            {
                                Physics2D.IgnoreCollision(currentBody.GetComponent<BoxCollider2D>(), vehicle.objects[j].GetComponent<Collider2D>(), true);
                            }

                            vehicle.objects[wheels+i] = currentBody.gameObject;
                            lastBody = currentBody;
                        }

                        //HOSE HEAD
                        HingeJoint2D hoseHead = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(8.7f * direction, -0.6f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        hoseHead.gameObject.layer = 13;
                        hoseHead.breakForce = 10000f;
                        hoseHead.transform.localScale = new Vector3(direction, 1f, 1f);
                        hoseHead.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path+"hoseHead.png");
                        hoseHead.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        hoseHead.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.1f);
                        hoseHead.connectedBody = lastBody.GetComponent<Rigidbody2D>();
                        hoseHead.anchor = new Vector2(-0.25f, 0f);
                        vehicle.objects[wheels+14] = hoseHead.gameObject;

                        foreach (Collider2D c in Instance.GetComponents<Collider2D>())
                        {
                            Physics2D.IgnoreCollision(hoseHead.GetComponent<Collider2D>(), c, true);
                        }
                        for (int i = 0; i < 15; i++)
                        {
                            Physics2D.IgnoreCollision(hoseHead.GetComponent<Collider2D>(), vehicle.objects[i].GetComponent<Collider2D>(), true);
                        }

                        //HOSE BEHAVIOUR
                        FlamethrowerBehaviour e = GameObject.Instantiate(ModAPI.FindSpawnable("Fire Extinguisher").Prefab, Vector3.zero, Quaternion.identity).GetComponent<FlamethrowerBehaviour>();
                        FlamethrowerBehaviour extinguisher = hoseHead.gameObject.AddComponent<FlamethrowerBehaviour>();
                        GameObject prt = GameObject.Instantiate(e.particlePrefab, Vector3.up * -10000, Quaternion.identity);
                        var main = prt.GetComponent<ParticleSystem>().main;
                        main.startSize = 2.5f;
                        main.startSpeed = 1.5f;
                        var sz = prt.GetComponent<ParticleSystem>().sizeOverLifetime;
                        sz.enabled = false;
                        var col = prt.GetComponent<ParticleSystem>().collision;
                        col.collidesWith = 5 << 9;//collides with the bounds and objects but nothing else (layer 9 is object and 11 is bounds, 5 in binary is 101 so bitshifted to the left 9 times will enable the 9th and the 11th)

                        extinguisher.particlePrefab = prt;
                        extinguisher.Effect = e.Effect;
                        extinguisher.LayerMask = e.LayerMask;
                        extinguisher.Point = e.Point;
                        extinguisher.Range = e.Range * 10;
                        extinguisher.Angle = e.Angle;

                        GameObject.Destroy(e.gameObject);

                        Transform muzzle = new GameObject().transform;
                        muzzle.parent = hoseHead.transform;
                        muzzle.localPosition = Vector3.zero;
                        extinguisher.Muzzle = muzzle;

                        Shooter shooter = hoseHead.gameObject.AddComponent<Shooter>();
                        shooter.direction = direction;
                        hoseHead.gameObject.AddComponent<UseEventTrigger>().Action = () => { shooter.shoot = true; };

                        AudioSource asrc = hoseHead.gameObject.AddComponent<AudioSource>();
                        asrc.loop = true;
                        asrc.clip = ModAPI.LoadSound(path+"hose.wav");
                        shooter.asrc = asrc;

                        //HOLDER
                        JointAngleLimits2D jointZero = new JointAngleLimits2D();
                        jointZero.min = 0; jointZero.max = 0;

                        HingeJoint2D holder = GameObject.Instantiate(w, Instance.transform.position + new Vector3(0.29f * direction, 0.35f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        holder.connectedBody = rb;
                        holder.limits = jointZero;
                        holder.GetComponent<SpriteRenderer>().sprite = null;
                        holder.GetComponent<CircleCollider2D>().radius = 0.1f;
                        vehicle.objects[wheels+15] = holder.gameObject;

                        //FIRE EXTINGUISHER
                        HingeJoint2D fireExtinguisher = GameObject.Instantiate(ModAPI.FindSpawnable("Fire Extinguisher").Prefab, Instance.transform.position + new Vector3(-5.75f * direction, -0.15f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        fireExtinguisher.transform.localScale = new Vector3(-direction, 1f, 1f);
                        fireExtinguisher.connectedBody = rb;
                        fireExtinguisher.breakForce = 500f;
                        JointAngleLimits2D limits = new JointAngleLimits2D();
                        limits.min = -5; limits.max = 5;
                        fireExtinguisher.limits = limits;
                        vehicle.objects[wheels+16] = fireExtinguisher.gameObject;

                        //LIGHTS
                        LightSprite red = ModAPI.CreateLight(Instance.transform, Color.red, 4, 0);
                        red.transform.localPosition = new Vector3(5.3f, 1.5f, 0f);
                        LightSprite blue = ModAPI.CreateLight(Instance.transform, Color.blue, 4, 0);
                        blue.transform.localPosition = new Vector3(4.7f, 1.5f, 0f);
                        PoliceLights pl = Instance.AddComponent<PoliceLights>();
                        pl.l1 = red;
                        pl.l2 = blue;

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -3300f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(3.2f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels + 17] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(5.2f, 0f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 18] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(5f * direction, -0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 19] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #endregion

            #region Super
            #region Bugatti Chiron
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Bugatti Chiron",
                    DescriptionOverride = "Blue.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Super/Chiron/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Super/Chiron/";
                        int wheels = 2;
                        float massMult = 1.2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.4f, -0.4f), new Vector2(2.6f, -0.4f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.3f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.acceleration = 0.2f;
                        car.MotorSpeed = -4200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(1f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.4f * direction, -0.05f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Ferrari 458
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Ferrari 458",
                    DescriptionOverride = "Fast.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Super/Ferrari/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Super/Ferrari/";
                        int wheels = 2;
                        float massMult = 1.1f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.6f, -0.4f), new Vector2(2.25f, -0.4f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.3f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.acceleration = 0.2f;
                        car.MotorSpeed = -4200f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(1f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.4f * direction, -0.05f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Lamborghini Aventador
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Lamborghini Aventador",
                    DescriptionOverride = "Yellow.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Super/Lamborghini/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Super/Lamborghini/";
                        int wheels = 2;
                        float massMult = 1.05f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.25f, -0.4f), new Vector2(2.3f, -0.4f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.3f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.acceleration = 0.17f;
                        car.MotorSpeed = -4250f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(1f, 0f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.4f * direction, -0.15f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Formula 1
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Formula 1 Car",
                    DescriptionOverride = "Fastest.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Super/F1/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Super/F1/";
                        int wheels = 2;
                        float massMult = 0.8f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        float direction = Instance.transform.localScale.x;
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.55f, -0.4f), new Vector2(2.1f, -0.4f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.acceleration = 0.25f;
                        vehicle.loopThreshold=60f;
                        car.MotorSpeed = -6500f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound(path+"loop.wav");
                        vehicle.start = ModAPI.LoadSound(path+"start.mp3");
                        vehicle.stop = ModAPI.LoadSound(path+"stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 1f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-1f * direction, -0.15f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Dragster
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Dragster",
                    DescriptionOverride = "High acceleration. Be careful around the engine.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Super/Dragster/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Super/Dragster/";
                        int wheels = 2;
                        float massMult = 1f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        float direction = Instance.transform.localScale.x;
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3.25f, -0.9f), new Vector2(4f, -0.9f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            if(i==0){wheel.transform.localScale*=1.25f;}
                            else{wheel.transform.localScale*=0.75f;}
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        vehicle.acceleration = 1f;
                        vehicle.loopThreshold = 200f;
                        vehicle.maxTorque=1000f;
                        car.MotorSpeed = -5000f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound(path + "loop.wav");
                        vehicle.start = ModAPI.LoadSound(path + "start.mp3");
                        vehicle.stop = ModAPI.LoadSound(path + "stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 1f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-1.75f * direction, -0.5f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels] = engine.gameObject;
                        
                        Transform flames = GameObject.Instantiate(ModAPI.FindSpawnable("Flamethrower").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).transform;
                        GameObject.Destroy(flames.GetComponent<SpriteRenderer>());
                        flames.localScale=new Vector3(direction,1f,1f);
                        foreach(Collider2D c in flames.GetComponents<Collider2D>()){GameObject.Destroy(c);}
                        flames.localEulerAngles=new Vector3(0f,0f,115f*direction);
                        HingeJoint2D hj=flames.gameObject.AddComponent<HingeJoint2D>();
                        hj.connectedBody=engine.GetComponent<Rigidbody2D>();
                        hj.limits=engineLimits;
                        DragsterFlames df = flames.gameObject.AddComponent<DragsterFlames>();
                        df.pb = flames.GetComponent<PhysicalBehaviour>();
                        df.car = car;
                        vehicle.objects[wheels+1]=flames.gameObject;

                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);
                            engine.gameObject.layer = 10;

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #endregion

            #region Civilian
            #region Ford Escape SUV
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Ford Escape SUV",
                    DescriptionOverride = "Dark red.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Civilian/Ford/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Civilian/Ford/";
                        int wheels = 2;
                        float massMult = 1.2f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.35f, -0.6f), new Vector2(2.35f, -0.6f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.3f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -3800f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-0.25f, -0.2f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(1.1f, -0.2f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.3f * direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region BMW E90
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "BMW E90",
                    DescriptionOverride = "It is a car.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Civilian/BMW/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Civilian/BMW/";
                        int wheels = 2;
                        float massMult = 1.1f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-1.9f, -0.75f), new Vector2(2.35f, -0.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.1f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -4500f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-0.5f, -0.2f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(0.85f, -0.2f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.15f * direction, -0.2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Jeep Wrangler
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Jeep Wrangler",
                    DescriptionOverride = "peeJ",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Civilian/Jeep/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Civilian/Jeep/";
                        int wheels = 2;
                        float massMult = 1f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-1.85f, -0.75f), new Vector2(2.05f, -0.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.4f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -4000f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(0.5f, 0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(1.5f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Toyota Corolla
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Toyota Corolla",
                    DescriptionOverride = "Drives forward.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Civilian/Corolla/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Civilian/Corolla/";
                        int wheels = 2;
                        float massMult = 1.1f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2f, -0.75f), new Vector2(2.45f, -0.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.15f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -4100f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-0.5f, -0.2f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(0.85f, -0.2f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(2.4f * direction, -0.2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Chevy Silverado
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Chevy Silverado",
                    DescriptionOverride = "Pickup truck, put things in the back.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Civilian/Chevy/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Civilian/Chevy/";
                        int wheels = 2;
                        float massMult = 1.5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.5f, -0.75f), new Vector2(3.1f, -0.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 4];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.4f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -4000f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //BACK DOOR
                        HingeJoint2D backDoor = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position+new Vector3(0f,-0.005f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        backDoor.transform.localScale = new Vector3(direction, 1f, 1f);
                        backDoor.GetComponent<Rigidbody2D>().gravityScale = 0f;
                        backDoor.GetComponent<Rigidbody2D>().angularDrag = 99999999f;
                        backDoor.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "backDoor.png");
                        backDoor.gameObject.FixColliders();
                        backDoor.connectedBody = rb;
                        backDoor.anchor = new Vector2(-3.8f, -0.15f);
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? 0f : -90f;
                        jal.max = direction < 0 ? 90f : 0f;
                        backDoor.limits = jal;
                        ToggleJointMotor tjm = backDoor.gameObject.AddComponent<ToggleJointMotor>();
                        tjm.speed = 15f * direction;
                        tjm.maxTorque = 1000f;
                        vehicle.objects[wheels] = backDoor.gameObject;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(2f, 0.5f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels+1] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(0.5f, 0.5f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 2] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(3.25f * direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 3] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #endregion

            #region Miscellaneous
            #region Lawn Mower
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Lawn Mower",
                    DescriptionOverride = "Mowes the lawn.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/LawnMower/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/LawnMower/";
                        int wheels = 2;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        rb.mass = 500f;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-0.4f, -0.45f), new Vector2(1.2f, -0.55f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 1];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 0.8f;
                            if (i == 1) { wheel.transform.localScale *= 0.7f; }
                            wheel.GetComponent<Rigidbody2D>().mass = 25f;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //BLADE
                        HingeJoint2D blade = GameObject.Instantiate(ModAPI.FindSpawnable("Knife").Prefab, Instance.transform.position + new Vector3(0.5f * direction, -0.55f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        blade.GetComponent<PhysicalBehaviour>().Properties.HardImpact = null;
                        blade.GetComponent<PhysicalBehaviour>().Selectable = false;
                        blade.transform.parent = Instance.transform;
                        blade.connectedBody = rb;
                        blade.connectedAnchor = new Vector2(0.5f, -0.55f);
                        JointAngleLimits2D limits = new JointAngleLimits2D();
                        limits.min = 90f;
                        limits.max = 270f;
                        blade.limits = limits;
                        JointMotor2D motor = new JointMotor2D();
                        motor.maxMotorTorque = 100000f;
                        motor.motorSpeed = -3500f;
                        blade.useMotor = true;
                        blade.motor = motor;
                        blade.gameObject.AddComponent<LawnMower>();

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -400f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.loopThreshold = 3f;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound(path + "loop.wav");
                        vehicle.start = ModAPI.LoadSound(path + "start.mp3");
                        vehicle.stop = ModAPI.LoadSound(path + "stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;
                    }
                }
            );
            #endregion
            #region Limousine
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Limousine",
                    DescriptionOverride = "Long.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/Limousine/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/Limousine/";
                        int wheels = 2;
                        float massMult = 1.5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-5.9f, -0.7f), new Vector2(6.25f, -0.7f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 1.5f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        car.MotorSpeed = -3750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("CarSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("CarSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("CarSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-4f, -0.2f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(4f, -0.2f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(6.25f * direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Monster Truck
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Monster Truck",
                    DescriptionOverride = "Monstrous truck.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/Monster/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/Monster/";
                        int wheels = 2;
                        float massMult = 5f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-3f, -0.7f), new Vector2(2.8f, -0.7f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 2.7f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.5f;
                            js.frequency = 4f;
                            wj.suspension = js;
                            wj.breakForce = 35000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        vehicle.acceleration = 0.2f;
                        car.MotorSpeed = -3750f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound("TruckSfx/loop.wav");
                        vehicle.start = ModAPI.LoadSound("TruckSfx/start.mp3");
                        vehicle.stop = ModAPI.LoadSound("TruckSfx/stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(2f, 1f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(3f * direction, 0.9f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f);
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Covered Wagon
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Stick"),
                    NameOverride = "Covered Wagon",
                    DescriptionOverride = "Doesn't drive forward.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/Wagon/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/Wagon/";
                        int wheels = 2;
                        float massMult = 4f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wooden Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-2.3f, -0.8f), new Vector2(0.25f, -0.8f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();

                        vehicle.objects = new GameObject[wheels + 1];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 1f;
                            js.frequency = 15f;
                            wj.suspension = js;
                            wj.breakForce = 2500f;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //HANDLE
                        HingeJoint2D handle = GameObject.Instantiate(ModAPI.FindSpawnable("Stick").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        handle.gameObject.layer=10;
                        handle.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "handle.png");
                        handle.gameObject.FixColliders();
                        handle.transform.localScale = new Vector3(direction, 1f, 1f);
                        handle.connectedBody = rb;
                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction < 0 ? -20 : -70;
                        jal.max = direction < 0 ? 70 : 20;
                        handle.limits=jal;
                        handle.anchor = new Vector2(0.8f, -0.9f);
                        handle.breakForce = 1500f;
                        vehicle.objects[wheels] = handle.gameObject;

                        //CONNECTOR
                        GameObject connector = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, handle.transform.position + new Vector3(3f * direction, -1f, 0f), Quaternion.identity);
                        connector.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("locator.png");
                        connector.transform.parent = handle.transform;
                        connector.transform.localScale = new Vector3(3f, 1f, 0f);
                        connector.FixColliders();
                        connector.GetComponent<SpriteRenderer>().sprite = null;
                        foreach (Collider2D c in connector.GetComponents<Collider2D>()) { c.isTrigger = true; }
                        connector.GetComponent<Rigidbody2D>().isKinematic = true;
                        connector.AddComponent<Connector>();
                    }
                }
            );
            #endregion
            #region Motorcycle
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Motorcycle",
                    DescriptionOverride = "It is also blue.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/Motorcycle/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/Motorcycle/";
                        int wheels = 2;
                        float massMult = 0.75f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-0.75f, -0.25f), new Vector2(0.92f, -0.25f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale *= 0.8f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.acceleration=0.001f;
                        vehicle.noMotorIndex=1;
                        car.MotorSpeed = -4500f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound(path+"loop.wav");
                        vehicle.start = ModAPI.LoadSound(path+"start.mp3");
                        vehicle.stop = ModAPI.LoadSound(path+"stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(0.2f * direction, -0.05f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*0.5f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Model T
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Model T",
                    DescriptionOverride = "It's old.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/ModelT/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/ModelT/";
                        int wheels = 2;
                        float massMult = 1.25f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-1.9f, -0.8f), new Vector2(1.9f, -0.8f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 3];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<CircleCollider2D>().radius*=1.25f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path+"wheel.png");
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0.75f;
                            js.frequency = 5f;
                            wj.suspension = js;
                            wj.breakForce = 15000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        vehicle.noMotorIndex = 1;
                        //vehicle.acceleration=0.01f;
                        vehicle.loopThreshold=21f;
                        car.MotorSpeed = -2000f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound(path+"loop.wav");
                        vehicle.start = ModAPI.LoadSound(path+"start.mp3");
                        vehicle.stop = ModAPI.LoadSound(path+"stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 0.75f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(-0.75f, 0.15f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //DOOR 2
                        HingeJoint2D door2 = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door2.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door2.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door2.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door2.png");
                        door2.gameObject.FixColliders();
                        door2.gameObject.layer = 10;
                        door2.transform.localScale = new Vector3(direction, 1f, 1f);
                        door2.connectedBody = rb;
                        door2.gameObject.AddComponent<Door>().direction = direction;
                        door2.anchor = new Vector2(0.5f, 0.15f);
                        door2.breakForce = 12000f;
                        vehicle.objects[wheels + 1] = door2.gameObject;
                        door2.gameObject.AddComponent<JointBreak>().action = () => { door2.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door2.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(1.25f * direction, 0f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*0.8f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 2] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Amtrak Train
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Amtrak Train",
                    DescriptionOverride = "Supposed to drive on rails.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/Train/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/Train/";
                        int wheels = 4;
                        float massMult = 15f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x;
                        SpriteRenderer background = new GameObject().AddComponent<SpriteRenderer>();
                        background.transform.localScale = new Vector3(direction, 1f, 1f);
                        background.transform.parent = Instance.transform;
                        background.transform.localPosition = Vector3.zero;
                        background.sprite = ModAPI.LoadSprite(path + "background.png");
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        //WHEELS
                        GameObject w = ModAPI.FindSpawnable("Metal Wheel").Prefab;

                        Vector2[] wps = { new Vector2(-6f, -1.75f), new Vector2(-3.5f, -1.75f), new Vector2(4f, -1.75f), new Vector2(6.5f, -1.75f) };

                        Vehicle vehicle = Instance.AddComponent<Vehicle>();
                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[wheels];

                        vehicle.objects = new GameObject[wheels + 2];
                        vehicle.wheels = new WheelJoint2D[wheels];

                        for (int i = 0; i < wheels; i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position + new Vector3(wps[i].x * direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite(path+"wheel.png");
                            wheel.transform.localScale*=1.5f;
                            wheel.GetComponent<CircleCollider2D>().radius*=0.7f;
                            wheel.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                            wheel.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                            wheel.GetComponent<Rigidbody2D>().mass *= massMult;
                            wheel.GetComponent<SpriteRenderer>().sortingOrder = 1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody = wheel.GetComponent<Rigidbody2D>();
                            wj.anchor = wps[i];
                            wj.autoConfigureConnectedAnchor = true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio = 0f;
                            js.frequency = 25f;
                            wj.suspension = js;
                            wj.breakForce = 50000f;
                            car.WheelJoints[i] = wj;
                            vehicle.wheels[i] = wj;
                            vehicle.objects[i] = wheel;
                        }

                        //CAR BEHAVIOUR
                        car.MotorSpeed = -5555f;
                        vehicle.loopThreshold=38f;
                        car.Activated = false;
                        car.Phys = Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged = true;

                        vehicle.car = car;
                        vehicle.source = Instance.AddComponent<AudioSource>();
                        vehicle.loop = ModAPI.LoadSound(path+"loop.wav");
                        vehicle.start = ModAPI.LoadSound(path+"start.mp3");
                        vehicle.stop = ModAPI.LoadSound(path+"stop.mp3");
                        vehicle.source.loop = true;
                        vehicle.source.volume = 1f;
                        vehicle.source.minDistance = 0.1f;
                        vehicle.source.maxDistance = 1f;

                        //DOOR
                        HingeJoint2D door = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position, Quaternion.identity).AddComponent<HingeJoint2D>();
                        door.GetComponent<Rigidbody2D>().angularDrag = 99999999999999f;
                        door.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                        door.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "door.png");
                        door.gameObject.FixColliders();
                        door.gameObject.layer = 10;
                        door.transform.localScale = new Vector3(direction, 1f, 1f);
                        door.connectedBody = rb;
                        door.gameObject.AddComponent<Door>().direction = direction;
                        door.anchor = new Vector2(7f, 0.4f);
                        door.breakForce = 12000f;
                        vehicle.objects[wheels] = door.gameObject;
                        door.gameObject.AddComponent<JointBreak>().action = () => { door.GetComponent<Rigidbody2D>().angularDrag = 0.05f; door.GetComponent<Rigidbody2D>().gravityScale = 1f; };

                        //ENGINE
                        HingeJoint2D engine = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-3.6f * direction, 0.25f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        engine.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("engine.png");
                        engine.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bottom");
                        engine.gameObject.FixColliders();
                        engine.gameObject.layer = 10;
                        engine.transform.localScale = new Vector3(direction, 1f, 1f)*1.33f;
                        engine.connectedBody = rb;
                        JointAngleLimits2D engineLimits = new JointAngleLimits2D();
                        engineLimits.min = 0f;
                        engineLimits.max = 0f;
                        engine.limits = engineLimits;
                        engine.breakForce = 25000f;
                        vehicle.objects[wheels + 1] = engine.gameObject;
                        engine.gameObject.AddComponent<JointBreak>().action = () =>
                        {
                            car.Activated = false;
                            GameObject.Destroy(car, 1f);

                            Rigidbody2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, engine.transform.position + new Vector3(0f, -0.25f, 0f), Quaternion.identity).GetComponent<Rigidbody2D>();
                            smoke.isKinematic = true;
                            GameObject.Destroy(smoke.GetComponent<Collider2D>());
                            smoke.GetComponent<ParticleMachineBehaviour>().Activated = true;
                            smoke.GetComponent<SpriteRenderer>().sprite = null;
                            smoke.transform.parent = engine.transform;
                        };
                    }
                }
            );
            #endregion
            #region Saturn V Rocket
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Saturn V Rocket",
                    DescriptionOverride = "Very large rocket.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("Miscellaneous/Rocket/preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        string path = "Miscellaneous/Rocket/";
                        float massMult = 100f;

                        //BODY
                        Rigidbody2D rb = Instance.GetComponent<Rigidbody2D>();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "col.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(path + "body.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        float direction = Instance.transform.localScale.x>0?1f:-1f;
                        Instance.GetComponent<PhysicalBehaviour>().TrueInitialMass *= massMult;
                        Instance.GetComponent<PhysicalBehaviour>().InitialMass *= massMult;
                        rb.mass *= massMult;

                        Instance.transform.localScale*=3f;

                        JointAngleLimits2D zero = new JointAngleLimits2D();
                        zero.min = 0f;
                        zero.max = 0f;

                        Transform flames = GameObject.Instantiate(ModAPI.FindSpawnable("Flamethrower").Prefab, Instance.transform.position + new Vector3(-1f, -38f, 0f), Quaternion.identity).transform;
                        flames.GetComponent<FlamethrowerBehaviour>().Range*=5f;
                        flames.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        GameObject.Destroy(flames.GetComponent<SpriteRenderer>());
                        flames.localScale = new Vector3(1f, 1f, 1f)*6f;
                        foreach (Collider2D c in flames.GetComponents<Collider2D>()) { GameObject.Destroy(c); }
                        flames.localEulerAngles = new Vector3(0f, 0f, -90f);
                        HingeJoint2D hj = flames.gameObject.AddComponent<HingeJoint2D>();
                        hj.connectedBody = Instance.GetComponent<Rigidbody2D>();
                        hj.limits = zero;

                        Transform flamesL = GameObject.Instantiate(ModAPI.FindSpawnable("Flamethrower").Prefab, Instance.transform.position + new Vector3(-3.5f, -40f, 0f), Quaternion.identity).transform;
                        flamesL.GetComponent<FlamethrowerBehaviour>().Range *= 5f;
                        flamesL.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        GameObject.Destroy(flamesL.GetComponent<SpriteRenderer>());
                        flamesL.localScale = new Vector3(1f, 1f, 1f) * 6f;
                        foreach (Collider2D c in flamesL.GetComponents<Collider2D>()) { GameObject.Destroy(c); }
                        flamesL.localEulerAngles = new Vector3(0f, 0f, -90f);
                        HingeJoint2D hjL = flamesL.gameObject.AddComponent<HingeJoint2D>();
                        hjL.connectedBody = Instance.GetComponent<Rigidbody2D>();
                        hjL.limits = zero;

                        Transform flamesR = GameObject.Instantiate(ModAPI.FindSpawnable("Flamethrower").Prefab, Instance.transform.position + new Vector3(2f, -40f, 0f), Quaternion.identity).transform;
                        flamesR.GetComponent<FlamethrowerBehaviour>().Range *= 5f;
                        flamesR.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        GameObject.Destroy(flamesR.GetComponent<SpriteRenderer>());
                        flamesR.localScale = new Vector3(1f, 1f, 1f) * 6f;
                        foreach (Collider2D c in flamesR.GetComponents<Collider2D>()) { GameObject.Destroy(c); }
                        flamesR.localEulerAngles = new Vector3(0f, 0f, -90f);
                        HingeJoint2D hjR = flamesR.gameObject.AddComponent<HingeJoint2D>();
                        hjR.connectedBody = Instance.GetComponent<Rigidbody2D>();
                        hjR.limits = zero;

                        Rocket rocket = Instance.gameObject.AddComponent<Rocket>();
                        rocket.effects=new PhysicalBehaviour[3];
                        rocket.effects[0] = flames.GetComponent<PhysicalBehaviour>();
                        rocket.effects[1] = flamesL.GetComponent<PhysicalBehaviour>();
                        rocket.effects[2] = flamesR.GetComponent<PhysicalBehaviour>();
                        rocket.source=Instance.AddComponent<AudioSource>();
                        rocket.loop = ModAPI.LoadSound(path+"loop.wav");

                        Transform bomb = GameObject.Instantiate(ModAPI.FindSpawnable("Atom Bomb").Prefab, Instance.transform.position, Quaternion.identity).transform;
                        GameObject.Destroy(bomb.GetComponent<SpriteRenderer>());
                        HingeJoint2D bhj = bomb.gameObject.AddComponent<HingeJoint2D>();
                        bhj.connectedBody = Instance.GetComponent<Rigidbody2D>();
                        bhj.limits = zero;
                        rocket.bomb=bomb.GetComponent<PhysicalBehaviour>();
                    }
                }
            );
            #endregion
            #endregion
        }
    }

    #region Classes
    public class Vehicle : MonoBehaviour
    {
        public GameObject[] objects;

        public WheelJoint2D[] wheels;

        public CarBehaviour car;

        public AudioSource source;
        public AudioClip loop;
        public AudioClip start;
        public AudioClip stop;

        public float acceleration = 0.05f;
        public float maxTorque = 300f;
        public float loopThreshold=0f;//Will be auto set if not pre defined
        public int noMotorIndex = -1;//disabled by default

        bool looping = false;
        bool stopped = false;
        bool started = false;

        float speed = 15f;

        void Start(){if(loopThreshold!=0f){return;}loopThreshold=speed+(160*acceleration);}

        void Update()
        {
            if (car != null && car.Activated)
            {
                if(!started)
                {
                    source.clip = start;
                    source.Play();
                    source.loop = false;
                    stopped = false;
                    started = true;
                }
                
                if (speed < maxTorque) { speed+=acceleration; }

                for(int i=0; i<wheels.Length; i++)
                {
                    if(i==noMotorIndex){wheels[i].useMotor=false;}//prevent car from tipping over when breaking
                    JointMotor2D jm = wheels[i].motor;
                    jm.maxMotorTorque = speed;
                    wheels[i].motor = jm;
                }

                if (!looping && speed > loopThreshold)//the speed > is just a means of giving the start sound time to play before the loop starts.
                {
                    source.clip=loop;
                    source.Play();
                    source.loop=true;
                    looping = true;
                }
            }
            else if(!stopped)
            {
                source.clip=stop;
                source.Play();
                source.loop=false;
                looping = false;
                started=false;
                stopped=true;
                speed = 15f;
            }
        }

        void OnDestroy()
        {
            foreach (GameObject o in objects) { GameObject.Destroy(o); }
        }
    }

    public class Door : MonoBehaviour
    {
        public float direction;
        
        HingeJoint2D hj;
        PhysicalBehaviour pb;
        JointAngleLimits2D doorLimits;
        Rigidbody2D rb;
        void Start()
        {
            rb=GetComponent<Rigidbody2D>();
            hj=GetComponent<HingeJoint2D>();
            pb=GetComponent<PhysicalBehaviour>();
            GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
            doorLimits=hj.limits;
            doorLimits.min = 0f;
            doorLimits.max = 0f;
            hj.limits=doorLimits;
        }
        void Update()
        {
            if(rb.velocity.magnitude > 50 || DragTool.GetHeldObject()==pb)
            {
                doorLimits.min = direction < 0 ? 0f : 20f;
                doorLimits.max = direction < 0 ? -20f : 0f;
                hj.limits=doorLimits;
                GameObject.Destroy(this);
            }
        }
    }

    public class SlideDoor : MonoBehaviour
    {
        SliderJoint2D sj;
        PhysicalBehaviour pb;
        JointTranslationLimits2D doorLimits;
        Rigidbody2D rb;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sj = GetComponent<SliderJoint2D>();
            pb = GetComponent<PhysicalBehaviour>();
            doorLimits = sj.limits;
            doorLimits.min = 0f;
            doorLimits.max = 0f;
            sj.limits = doorLimits;
        }
        void Update()
        {
            if (rb.velocity.magnitude > 50 || DragTool.GetHeldObject() == pb)
            {
                doorLimits.max = 1.5f;
                sj.limits = doorLimits;
                GameObject.Destroy(this);
            }
        }
    }

    public class JointBreak : MonoBehaviour
    {
        public delegate void Del();
        public Del action;
        void OnJointBreak2D(Joint2D brokenJoint) { action(); }
    }

    public class ToggleJointMotor : MonoBehaviour
    {
        public float speed = 2f;
        public float maxTorque = 100000000f;
        
        HingeJoint2D hj;
        JointMotor2D motor;

        void Start()
        {
            hj=GetComponent<HingeJoint2D>();
            motor=new JointMotor2D();
            motor.motorSpeed=speed;
            motor.maxMotorTorque=maxTorque;
            hj.motor=motor;

            gameObject.AddComponent<UseEventTrigger>().Action = this.Toggle;
        }

        public void Toggle()
        {
            motor.motorSpeed*=-1f;
            hj.motor=motor;
        }
    }

    public class Connector : MonoBehaviour
    {
        HingeJoint2D hj;

        void Start()
        {
            transform.parent.gameObject.AddComponent<UseEventTrigger>().Action = () => { GameObject.Destroy(hj); };
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if(hj==null && other.GetComponent<AttachPoint>()!=null)
            {
                hj=transform.parent.gameObject.AddComponent<HingeJoint2D>();
                hj.connectedBody=other.transform.parent.GetComponent<Rigidbody2D>();
                hj.anchor = new Vector2(7f, -1.5f);
                JointAngleLimits2D jal = new JointAngleLimits2D();
                jal.min = -6f;
                jal.max = 6f;
                hj.limits=jal;
                hj.breakForce=25000f;
            }
        }
    }

    public class AttachPoint : MonoBehaviour{}

    public class Hook : MonoBehaviour
    {
        Rigidbody2D rb;
        UseEventTrigger use;
        FixedJoint2D fj;

        Rigidbody2D otherRb;
        PhysicalBehaviour pb;

        bool attached = false;
        bool disableUse = false;
        float time = 0f;
        float massHelp = 5f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if(other.gameObject.layer!=9){return;}
            use=gameObject.AddComponent<UseEventTrigger>();
            use.Action = () =>
            {
                attached=!attached;
                if(attached)
                {
                    fj = gameObject.AddComponent<FixedJoint2D>();
                    otherRb = other.gameObject.GetComponent<Rigidbody2D>();
                    pb = otherRb.GetComponent<PhysicalBehaviour>();
                    fj.connectedBody = otherRb;
                    pb.TrueInitialMass /= massHelp;
                    pb.InitialMass /= massHelp;
                    otherRb.mass /= massHelp;
                }
                else
                {
                    GameObject.Destroy(fj);
                    GameObject.Destroy(use);

                    pb.TrueInitialMass *= massHelp;
                    pb.InitialMass *= massHelp;
                    otherRb.mass *= massHelp;
                }
            };
        }
    }

    public class DragsterFlames : MonoBehaviour
    {
        public PhysicalBehaviour pb;
        public CarBehaviour car;
        float time=1f;
        int count=0;
        bool start = false;
        bool canStart = true;
        void Update()
        {
            if(car.Activated && canStart) { start = true; canStart = false; }
            if(!car.Activated && !canStart) 
            { 
                canStart = true;
                start = false;
                count = 0;
                time = 1f;
            }
            if(!start) { return; }
            
            time+=Time.deltaTime;
            if(time>0.35f)
            {
                pb.UseContinuous(new ActivationPropagation());
                time=0f;
                count++;
                if(count>3){start=false;}
            }
        }
    }

    public class PoliceLights : MonoBehaviour
    {
        public LightSprite l1;
        public LightSprite l2;
        public float brightness = 15f;

        bool activated = false;
        float time = 0f;

        void Start()
        {
            gameObject.AddComponent<UseEventTrigger>().Action = () =>
            {
                activated = !activated;
                time=0f;

                if(activated)
                {
                    l1.Brightness = brightness;
                    l2.Brightness = 0f;
                }
                else
                {
                    l1.Brightness = 0f;
                    l2.Brightness = 0f;
                }
            };
        }

        void Update()
        {
            if(!activated){return;}

            if(time>0.5f)
            {
                l1.Brightness=l1.Brightness!=0?0:brightness;
                l2.Brightness=l2.Brightness!=0?0:brightness;
                time=0f;
            }

            time+=Time.deltaTime;
        }
    }

    public class LawnMower : MonoBehaviour
    {
        HingeJoint2D hj;
        JointMotor2D motor;

        bool recentChange = false;
        bool activated = false;

        void Start()
        {
            hj = GetComponent<HingeJoint2D>();
            motor = hj.motor;

            gameObject.AddComponent<UseEventTrigger>().Action = ()=>
            {
                activated=!activated;
                transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                recentChange = false;
                motor.motorSpeed = -3500f;
            };
        }

        void Update()
        {
            if(!activated){return;}
            
            if((transform.localEulerAngles.z > 269f || transform.localEulerAngles.z < 91f) && !recentChange)
            {
                motor.motorSpeed *= -1f;
                hj.motor = motor;
                recentChange=true;
            }
            else if(transform.localEulerAngles.z < 260 && transform.localEulerAngles.z > 100){recentChange=false;}
        }
    }

    public class ForkLift : MonoBehaviour
    {
        SliderJoint2D sj;
        JointMotor2D motor;

        void Start()
        {
            sj = GetComponent<SliderJoint2D>();
            motor=sj.motor;
            gameObject.AddComponent<UseEventTrigger>().Action=this.ToggleLift;
        }
        
        void ToggleLift()
        {
            motor.motorSpeed*=-1f;
            sj.motor=motor;
        }
    }

    public class Shooter : MonoBehaviour
    {
        public bool shoot = false;

        public float direction = 1f;

        public AudioSource asrc;

        bool playing = false;

        void Update()
        {
            if (shoot)
            {
                Rigidbody2D r = GameObject.Instantiate(ModAPI.FindSpawnable("Soap").Prefab, transform.position + transform.right, Quaternion.identity).GetComponent<Rigidbody2D>();
                r.gameObject.layer = 13;
                r.GetComponent<SpriteRenderer>().sprite = null;
                r.GetComponent<PhysicalBehaviour>().Properties.SlidingLoop = null;//remove sound when sliding
                r.GetComponent<PhysicalBehaviour>().Wetness = 1000000f;//help extinguish fires
                r.AddForce(transform.right * 0.75f * direction, ForceMode2D.Impulse);
                GameObject.Destroy(r.gameObject, 1f);

                if (!playing) { asrc.Play(); playing = true; }

                if (Input.GetKeyUp(KeyCode.F)) { shoot = false; }
            }
            else { asrc.Stop(); playing = false; }
        }
    }
    public class Boat : MonoBehaviour
    {
        public float speed = 100f;

        public AudioSource source;
        public AudioClip loop;
        public AudioClip start;
        public AudioClip stop;

        bool on = false;
        bool looping = false;
        float timer = 0f;

        Rigidbody2D rb;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            gameObject.AddComponent<UseEventTrigger>().Action = () => 
            { 
                on = !on;
                source.loop=false;
                timer=0f;
                if(on)
                {
                    source.clip=start;
                    source.Play();
                }
                else
                {
                    source.clip=stop;
                    source.Play();
                }
            };
        }

        void Update()
        {
            if (!on) { return; }

            timer+=Time.deltaTime;
            if(!looping && timer > 4f)
            {
                source.loop=true;
                source.clip=loop;
                source.Play();
                looping=true;
            }

            rb.AddForce(transform.right * speed * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    public class Rocket : MonoBehaviour
    {
        float maxSpeed = 75000f;
        float speed = 5000f;
        float acceleration = 50f;
        float explodeThreshold = 30f;
        bool activated = false;
        bool sound = false;

        Rigidbody2D rb;
        public PhysicalBehaviour[] effects;
        public AudioSource source;
        public AudioClip loop;
        public PhysicalBehaviour bomb;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            source.loop=true;
            source.clip = loop;
            gameObject.AddComponent<UseEventTrigger>().Action = () => 
            {
                activated=!activated;
                speed=5000f;
                sound=false;
                source.Stop();
            };
        }

        void Update()
        {
            if(!activated){return;}

            if(!sound)
            {
                source.Play();
                sound=true;
            }

            rb.AddForce(transform.up*speed*Time.deltaTime, ForceMode2D.Impulse);
            if(speed<maxSpeed){speed+=acceleration;}

            foreach(PhysicalBehaviour pb in effects)
            {
                pb.UseContinuous(new ActivationPropagation());
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if(rb.velocity.magnitude > explodeThreshold && other.gameObject.GetComponent<Rigidbody2D>().mass > 25f && activated)
            {
                bomb.ForceSendUse();
                GameObject.Destroy(gameObject, 0.1f);
            }
        }
    }
    #endregion
}