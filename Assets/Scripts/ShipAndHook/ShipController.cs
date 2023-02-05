using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipController : MonoBehaviour
{
    public HookController m_HookL;
    public HookController m_HookR;

    public Ship m_Owner;

    [Header("Sound Effect")]
    public AudioSource m_EngineStartAudioSource;
    public AudioSource m_EngineRunningAudioSource;

    [Header("Partical Effect GO")]
    public GameObject m_GOTLEnginePE;
    public GameObject m_GOTREnginePE;
    public GameObject m_GOBLEnginePE;
    public GameObject m_GOBREnginePE;

    [Header("Rope")]
    public GameObject leftRopeStartGo;
    public GameObject rightRopeStartGo;
    public GameObject leftRopeEndGo;
    public GameObject rightRopeEndGo;

    public float m_AngularTurnSpeed = 10.0f;

    // not debug, give ship an init speed
    public Vector3 m_initSpeed = Vector3.zero;
    [ReadOnly]
    public Vector3 m_Velocity = Vector3.zero;

    public StateMachine m_StateMachine = new StateMachine();
    private Dictionary<HookType, HookController> m_HookLookup = new Dictionary<HookType, HookController>();

    private Planet m_HookGrabbedPlanet = null;
    private bool m_TriggerHookRelease = false;
    private Vector3 hookLInitLocalPos;
    private Vector3 hookRInitLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(m_StateMachine);
        Assert.IsNotNull(m_HookL);
        Assert.IsNotNull(m_HookR);
        m_Owner = GetComponent<Ship>();
        Assert.IsNotNull(m_Owner);
        Assert.IsNotNull(m_EngineStartAudioSource);
        Assert.IsNotNull(m_EngineRunningAudioSource);

        Assert.IsNotNull(m_GOTLEnginePE);
        Assert.IsNotNull(m_GOTREnginePE);
        Assert.IsNotNull(m_GOBLEnginePE);
        Assert.IsNotNull(m_GOBREnginePE);

        m_GOTLEnginePE.SetActive(false);
        m_GOTREnginePE.SetActive(false);
        m_GOBLEnginePE.SetActive(false);
        m_GOBREnginePE.SetActive(false);

        m_Velocity = m_initSpeed;

        m_HookLookup[HookType.Left] = m_HookL;
        m_HookLookup[HookType.Right] = m_HookR;

        m_StateMachine.Init(gameObject, new StateCruise());

        hookLInitLocalPos = m_HookL.transform.localPosition;
        hookRInitLocalPos = m_HookR.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        m_StateMachine.Update();

        if (!m_Owner.m_isWin) {
            leftRopeStartGo.transform.localPosition = hookLInitLocalPos;
            rightRopeStartGo.transform.localPosition = hookRInitLocalPos;
            leftRopeEndGo.transform.position = m_HookL.transform.position;
            rightRopeEndGo.transform.position = m_HookR.transform.position;
        }

        //if (Input.GetButtonDown("Left"))
        //{

        //}
        //else
        //{

        //}
    }

    private void OnDestroy()
    {
        m_StateMachine.CleanUp();
    }

    public void MoveShipInCruise()
    {
        transform.position = transform.position + m_Velocity * Time.deltaTime;
    }

    public void RotateShipByManualTurning(float degree)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, degree);
        transform.rotation = rotation * transform.rotation;

        m_Owner.ConsumeEnergyByTurning();
    }

    public void RotateHooks()
    {
        m_HookL.UpdateRot();
        m_HookR.UpdateRot();
    }

    public void RotateHookWithShip(HookType type)
    {
        m_HookLookup[type].UpdateRot();
    }

    public void MoveHookWithShip(HookType type)
    {
        m_HookLookup[type].UpdateFollowShipPos();
    }

    public void InitHookStateBeforeShoot(HookType type)
    {
        m_HookLookup[type].InitHookStateBeforeShoot();
    }

    public void InitHookStateBeforeRetrieve(HookType type)
    {
        m_HookLookup[type].InitHookStateBeforeRetrieve();
    }

    public void SwitchHookAnimState(HookType type, bool isHit)
    {
        m_HookLookup[type].SwitchHookAnimState(isHit);
    }

    public bool MoveShootHook(HookType type)
    {
        switch (type)
        {
            case HookType.Left:
                return m_HookL.UpdateShootHookPos(-transform.right);
            case HookType.Right:
                return m_HookR.UpdateShootHookPos(transform.right);
        }

        return false;
    }

    public bool MoveRetrievingHook(HookType type)
    {
        // simple handle
        switch (type)
        {
            case HookType.Left:
                return m_HookL.UpdateRetrieveHookPos(m_HookL.transform.position - transform.position);
            case HookType.Right:
                return m_HookR.UpdateRetrieveHookPos(m_HookR.transform.position - transform.position);
        }

        return false;
    }

    public Planet GetHookGrabbedPlanet() { return m_HookGrabbedPlanet; }
    public bool IsHookGrabbingOnPlanet() { return m_HookGrabbedPlanet != null; }

    public void OnHookGrabOnPlanet(Planet planet)
    {
        m_HookGrabbedPlanet = planet;
    }

    public void OnHookReleasePlanet()
    {
        // float radius = (m_HookGrabbedPlanet.transform.position - transform.position).magnitude;
        // int clockwiseFlag = m_HookGrabbedPlanet.IsClockwiseRotate() ? -1 : 1;
        // m_Velocity = clockwiseFlag * Mathf.Deg2Rad * m_HookGrabbedPlanet.m_PlanetController.m_AngularRotateSpeed * radius * transform.up;

        m_HookGrabbedPlanet.SetToDestroy();
        m_HookGrabbedPlanet = null;
    }

    public void LockHookAtPlanetCenter(HookType type)
    {
        m_HookLookup[type].transform.position = m_HookGrabbedPlanet.transform.position;
    }

    public void RotateToPlanetRotationTangent()
    {
        // RotateToGoRotationTangent(m_HookGrabbedPlanet.transform,
            // m_HookGrabbedPlanet.IsClockwiseRotate());
        Vector3 toShip = transform.position - m_HookGrabbedPlanet.transform.position;
        Vector3 up = Vector3.Cross(Vector3.forward, toShip);
        int clockwiseFlag = m_HookGrabbedPlanet.IsClockwiseRotate() ? -1 : 1;
        m_Velocity = clockwiseFlag * up;
    }

    public void RotateToGoRotationTangent(Transform trans, bool isClockwiseRotate) {
        Vector3 toShip = transform.position - trans.position;
        Vector3 up = Vector3.Cross(Vector3.forward, toShip);
        int clockwiseFlag = isClockwiseRotate ? -1 : 1;
        transform.up = clockwiseFlag * up;
    }

    public void MoveAndRotateWithPlanet()
    {
        MoveAndRotateWithGo(m_HookGrabbedPlanet.gameObject,
            m_HookGrabbedPlanet.m_PlanetController.m_AngularRotateSpeed);
    }

    public void MoveAndRotateWithGo(GameObject go, float speed)
    {
        Vector3 toShip = transform.position - go.transform.position;
        float rotateSpeed = speed;
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, rotateSpeed * Time.deltaTime);
        Matrix4x4 rotMat = Matrix4x4.Rotate(rotation);
        //Vector3 toShipNew = rotation * toShip;
        Vector3 toShipNew = rotMat.MultiplyVector(toShip);
        transform.position = toShipNew + go.transform.position;
        transform.rotation = (rotation * transform.rotation).normalized;
    }

    public void TriggerHookRelease()
    {
        m_TriggerHookRelease = true;
    }

    public bool CheckAndResetTriggerHookRelease()
    {
        if (m_TriggerHookRelease)
        {
            m_TriggerHookRelease = false;
            return true;
        }
        return false;
    }

    public void EnableLeftTurnEngines()
    {
        m_GOTREnginePE.SetActive(true);
        m_GOBLEnginePE.SetActive(true);
    }
    public void DisableLeftTurnEngines()
    {
        m_GOTREnginePE.SetActive(false);
        m_GOBLEnginePE.SetActive(false);
    }
    public void EnableRightTurnEngines()
    {
        m_GOTLEnginePE.SetActive(true);
        m_GOBREnginePE.SetActive(true);
    }
    public void DisableRightTurnEngines()
    {
        m_GOTLEnginePE.SetActive(false);
        m_GOBREnginePE.SetActive(false);
    }

    public void PlayEngineAudio()
    {
        m_EngineStartAudioSource.Play();
        m_EngineRunningAudioSource.Play();
    }

    public void StopEngineAudio()
    {
        m_EngineStartAudioSource.Stop();
        m_EngineRunningAudioSource.Stop();
    }
}

public enum HookType
{
    Left,
    Right,
    Invalid
}
public class StateCruise : State
{
    private ShipController m_Controller;

    public StateCruise()
    {
        StateId = StateIdEnum.StateCruise;
    }

    public override void OnEntrance(State lastState)
    {
        Debug.Log("Enter StateCruise");
        m_Controller = Parent.Owner.GetComponent<ShipController>();
    }

    public override State OnRun()
    {
        // Effects
        bool turnOn = false;
        if (Input.GetButtonDown("LeftTurn"))
        {
            m_Controller.EnableLeftTurnEngines();
            turnOn = true;
        }
        if (Input.GetButtonDown("RightTurn"))
        {
            m_Controller.EnableRightTurnEngines();
            turnOn = true;
        }
        if (turnOn)
        {
            m_Controller.PlayEngineAudio();
        }

        bool turnOff = false;
        if (Input.GetButtonUp("LeftTurn"))
        {
            m_Controller.DisableLeftTurnEngines();
            turnOff = true;
        }
        if (Input.GetButtonUp("RightTurn"))
        {
            m_Controller.DisableRightTurnEngines();
            turnOff = true;
        }
        if (turnOff)
        {
            m_Controller.StopEngineAudio();
        }

        // Turn
        float rotDegree = 0;
        if (Input.GetButton("LeftTurn"))
        {
            rotDegree = m_Controller.m_AngularTurnSpeed * Time.deltaTime;
        }
        if (Input.GetButton("RightTurn"))
        {
            rotDegree = -m_Controller.m_AngularTurnSpeed * Time.deltaTime;
        }
        if (rotDegree != 0)
        {
            m_Controller.RotateShipByManualTurning(rotDegree);
        }

        m_Controller.MoveShipInCruise();
        m_Controller.MoveHookWithShip(HookType.Left);
        m_Controller.MoveHookWithShip(HookType.Right);
        m_Controller.RotateHooks();

        if (Input.GetButtonDown("LeftHook"))
        {
            return new StateDeployHook(HookType.Left);
        }
        if (Input.GetButtonDown("RightHook"))
        {
            return new StateDeployHook(HookType.Right);
        }

        if (m_Controller.m_Owner.m_isWin) {
            return new StateWin();
        }

        return null;
    }

    public override void OnExit(State nextState)
    {
        m_Controller.DisableLeftTurnEngines();
        m_Controller.DisableRightTurnEngines();
        m_Controller.StopEngineAudio();
    }
}

public class StateDeployHook : State
{
    private ShipController m_Controller;
    private HookType m_HookType;

    public StateDeployHook(HookType type) : base()
    {
        m_HookType = type;
        StateId = StateIdEnum.StateDeployHook;
    }

    public override void OnEntrance(State lastState)
    {
        Debug.Log("Enter StateDeployHook");

        m_Controller = Parent.Owner.GetComponent<ShipController>();
        m_Controller.InitHookStateBeforeShoot(m_HookType);
    }

    public override State OnRun()
    {
        if (m_Controller.IsHookGrabbingOnPlanet())
        {
            return new StateHookLocked(m_HookType);
        }

        m_Controller.MoveShipInCruise();

        HookType hookFollowing = HookType.Invalid;
        HookType hookShoot = HookType.Invalid;

        if (m_HookType == HookType.Right)
        {
            hookFollowing = HookType.Left;
            hookShoot = HookType.Right;
        }
        if (m_HookType == HookType.Left)
        {
            hookFollowing = HookType.Right;
            hookShoot = HookType.Left;
        }

        m_Controller.MoveHookWithShip(hookFollowing);
        bool finished = m_Controller.MoveShootHook(hookShoot);
        m_Controller.RotateHooks();

        if (finished)
        {
            return new StateRetrieveHook(m_HookType);
        }

        if (m_Controller.m_Owner.m_isWin) {
            return new StateWin();
        }

        return null;
    }
}

public class StateRetrieveHook : State
{
    private ShipController m_Controller;
    private HookType m_HookType;

    public StateRetrieveHook(HookType type) : base()
    {
        m_HookType = type;
        StateId = StateIdEnum.StateRetrieveHook;
    }

    public override void OnEntrance(State lastState)
    {
        Debug.Log("Enter StateRetrieveHook");

        m_Controller = Parent.Owner.GetComponent<ShipController>();
        m_Controller.InitHookStateBeforeRetrieve(m_HookType);
        m_Controller.SwitchHookAnimState(m_HookType, false);
    }

    public override State OnRun()
    {
        if (m_Controller.IsHookGrabbingOnPlanet())
        {
            return new StateHookLocked(m_HookType);
        }

        m_Controller.MoveShipInCruise();

        HookType hookFollowing = HookType.Invalid;
        HookType hookRetrieve = HookType.Invalid;

        if (m_HookType == HookType.Right)
        {
            hookFollowing = HookType.Left;
            hookRetrieve = HookType.Right;
        }
        if (m_HookType == HookType.Left)
        {
            hookFollowing = HookType.Right;
            hookRetrieve = HookType.Left;
        }

        m_Controller.MoveHookWithShip(hookFollowing);
        bool finished = m_Controller.MoveRetrievingHook(hookRetrieve);
        m_Controller.RotateHooks();

        if (finished)
        {
            return new StateCruise();
        }

        if (m_Controller.m_Owner.m_isWin) {
            return new StateWin();
        }

        return null;
    }
}

public class StateHookLocked : State
{
    private ShipController m_Controller;
    private HookType m_HookType;

    public StateHookLocked(HookType type) : base()
    {
        m_HookType = type;
        StateId = StateIdEnum.StateHookLocked;
    }

    public override void OnEntrance(State lastState)
    {
        Debug.Log("Enter StateHookLocked");

        m_Controller = Parent.Owner.GetComponent<ShipController>();
        m_Controller.RotateToPlanetRotationTangent();
    }

    public override State OnRun()
    {
        if (m_Controller.CheckAndResetTriggerHookRelease()
            || Input.GetButtonDown("LeftHook")
            || Input.GetButtonDown("RightHook"))
        {
            return new StateRetrieveHook(m_HookType);
        }

        m_Controller.MoveAndRotateWithPlanet();

        HookType hookFollowing = HookType.Invalid;
        HookType hookLocked = HookType.Invalid;

        if (m_HookType == HookType.Right)
        {
            hookFollowing = HookType.Left;
            hookLocked = HookType.Right;
        }
        if (m_HookType == HookType.Left)
        {
            hookFollowing = HookType.Right;
            hookLocked = HookType.Left;
        }

        m_Controller.MoveHookWithShip(hookFollowing);
        m_Controller.LockHookAtPlanetCenter(hookLocked);
        m_Controller.RotateHooks();

        if (m_Controller.m_Owner.m_isWin) {
            return new StateWin();
        }
        return null;
    }

    public override void OnExit(State nextState)
    {
        m_Controller.OnHookReleasePlanet();
    }
}

public class StateWin : State {
    private ShipController m_Controller;
    private GameObject m_earthGo;
    private PlanetController m_earthController;

    public StateWin() : base() {
        StateId = StateIdEnum.StateWin;
    }

    public override void OnEntrance(State lastState)
    {
        Debug.Log("Enter Win");

        m_Controller = Parent.Owner.GetComponent<ShipController>();
        m_earthGo = m_Controller.m_Owner.m_earthGo;
        m_earthController = m_earthGo.GetComponent<PlanetController>();

        GameObject.Destroy(m_Controller.transform.Find("LineL")?.gameObject);
        GameObject.Destroy(m_Controller.transform.Find("LineL")?.gameObject);
        m_Controller.m_HookL.transform.localPosition = new Vector3(-0.8f, 0.25f, 0);
        m_Controller.m_HookR.transform.localPosition = new Vector3(0.8f, 0.25f, 0);

        m_Controller.RotateToGoRotationTangent(
            m_earthGo.transform, m_earthController.m_AngularRotateSpeed < 0);
    }

    public override State OnRun() {
        m_Controller.MoveAndRotateWithGo(
            m_earthGo, m_earthGo.GetComponent<PlanetController>().m_AngularRotateSpeed);

        return null;
    }
}