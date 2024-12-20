using Enums;
using HarmonyLib;
using UnityEngine;

public class PatchDebug
{
    [HarmonyPatch(typeof(MenuDebugWounds))]
    [HarmonyPatch(nameof(MenuDebugWounds.OnFoodPoisoningHealButton))]
    public class DebugWound_FoodPoisonHeal
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            FoodPoisoning foodpoison = (FoodPoisoning)PlayerDiseasesModule.Get().GetDisease(Enums.ConsumeEffect.FoodPoisoning);
            if (foodpoison != null)
            {
                foodpoison.m_Level = 0;
                foodpoison.m_LevelWithValue = 0f;
                foodpoison.Deactivate();
                foodpoison.DeactivateSymptoms();
            }
        }

    }

    [HarmonyPatch(typeof(MenuDebugWounds))]
    [HarmonyPatch(nameof(MenuDebugWounds.OnFeverHealButton))]
    public class DebugWound_FeverHeal
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            Fever fever = (Fever)PlayerDiseasesModule.Get().GetDisease(Enums.ConsumeEffect.Fever);
            if (fever != null)
            {
                fever.Deactivate();
                fever.m_Level = 0;
                fever.m_LevelWithValue = 0f;
                fever.DeactivateSymptoms();
            }
        }

    }

    [HarmonyPatch(typeof(MenuDebugWounds))]
    [HarmonyPatch(nameof(MenuDebugWounds.OnParasiteSicknesHealButton))]
    public class DebugWound_ParasiteHeal
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ParasiteSickness parasite = (ParasiteSickness)PlayerDiseasesModule.Get().GetDisease(Enums.ConsumeEffect.ParasiteSickness);
            if (parasite != null)
            {
                parasite.Deactivate();
                parasite.m_Level = 0;
                parasite.m_LevelWithValue = 0f;
                parasite.DeactivateSymptoms();
            }
        }

    }
    [HarmonyPatch(typeof(MainLevel), "Awake")]
    public class Init_Patch
    {
        [HarmonyPostfix]
        static void Init_Fix(MainLevel __instance)
        {
            //if (__instance.gameObject.GetComponent<AddHealthReplicator>())
            //{
            //    Object.Destroy(__instance.gameObject.GetComponent<AddHealthReplicator>());
            //}
            //if (__instance.gameObject.GetComponent<TeleportReplicator>())
            //{
            //    Object.Destroy(__instance.gameObject.GetComponent<TeleportReplicator>());
            //}
            if (!__instance.gameObject.GetComponent<AddHealthReplicator>())
            {
                __instance.gameObject.AddComponent<AddHealthReplicator>();
            }
            if (!__instance.gameObject.GetComponent<TeleportReplicator>())
            {
                __instance.gameObject.AddComponent<TeleportReplicator>();
            }
        }
    }

    [HarmonyPatch(typeof(FPPController), "UpdateWantedSpeed")]
    public class CameraManager_UpdateNormalMode
    {
        [HarmonyPrefix]
        static bool Prefix_NormalMode(FPPController __instance)
        {
            if (GreenHellMenu.Instance.isSkipAcceDefault)
            {
                if (!__instance.IsActive() || (FreeHandsLadderController.Get().IsActive() && !__instance.m_JumpInProgress) || LoadingScreen.Get().m_Active || StrokingController.Get().IsActive())
                {
                    __instance.m_WantedSpeed.Reset();
                    __instance.m_SlidingSpeedSoft.Reset();
                    __instance.m_SlideDir = Vector3.zero;
                    __instance.m_CurentMoveSpeed = 0f;
                    __instance.m_CurrentMoveSpeedType = MoveSpeed.Idle;
                    return false;
                }
                Vector2 vector = new Vector2(__instance.m_Inputs.m_Horizontal, __instance.m_Inputs.m_Vertical);
                if (vector.sqrMagnitude > 1f)
                {
                    vector.Normalize();
                }
                bool flag = (__instance.m_CollisionFlags & CollisionFlags.Below) == CollisionFlags.None;
                bool flag2 = !flag && (__instance.m_CollisionFlagsAddSpeed & CollisionFlags.Below) == CollisionFlags.None;
                bool flag3 = __instance.m_Inputs.m_Sprint && __instance.CanSprint();
                float num = 0f;
                __instance.m_CurrentMoveSpeedType = MoveSpeed.Idle;
                bool flag4 = __instance.m_Player && __instance.m_Player.GetMovesBlocked();
                if (__instance.m_RunDepletedStamina)
                {
                    if (flag3)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        __instance.m_RunDepletedStamina = false;
                    }
                }
                if (BowController.Get().m_MaxAim)
                {
                    flag3 = false;
                }
                if (flag3 && __instance.m_RunBlocked)
                {
                    flag3 = false;
                }
                if (!flag4 && vector.magnitude > 0.5f)
                {
                    if (flag3)
                    {
                        if (vector.y < -0.5f)
                        {
                            num = __instance.m_BackwardWalkSpeed;
                            __instance.m_CurrentMoveSpeedType = MoveSpeed.Walk;
                        }
                        else
                        {
                            num = __instance.m_RunSpeed;
                            __instance.m_CurrentMoveSpeedType = MoveSpeed.Run;
                        }
                    }
                    else if (vector.magnitude > 0.5f)
                    {
                        num = ((vector.y >= 0f) ? __instance.m_WalkSpeed : (Player.Get().HasSleighAttached() ? 0f : __instance.m_BackwardWalkSpeed));
                        __instance.m_CurrentMoveSpeedType = MoveSpeed.Walk;
                    }
                    if (__instance.m_Player && __instance.m_Player.IsStaminaDepleted())
                    {
                        num = __instance.m_WalkSpeed;
                        __instance.m_CurrentMoveSpeedType = MoveSpeed.Walk;
                        if (flag3)
                        {
                            __instance.m_RunDepletedStamina = true;
                        }
                    }
                }
                if (__instance.m_Duck)
                {
                    num *= __instance.m_DuckSpeedMul;
                }
                Vector3 vector2 = __instance.m_CharacterController.transform.InverseTransformVector(__instance.m_WantedSpeed.target);
                vector2.x = vector.x * num;
                vector2.z = vector.y * num;
                if (__instance.m_Dodge)
                {
                    num = 10f;
                    if (__instance.m_DodgeDirection != Direction.Backward)
                    {
                        vector2.x = ((__instance.m_DodgeDirection == Direction.Right) ? 1f : -1f);
                        vector2.z = 0f;
                    }
                    else
                    {
                        vector2.x = 0f;
                        vector2.z = -1f;
                    }
                    if (Time.time - __instance.m_DodgeStartTime > 0.15f)
                    {
                        __instance.m_Dodge = false;
                    }
                }
                float y = vector2.y;
                vector2.y = 0f;
                vector2.Normalize();
                vector2 *= num;
                vector2.y = y;
                if (InventoryBackpack.Get().IsMaxOverload())
                {
                    vector2.z *= __instance.m_MaxOverloadSpeedMul;
                    vector2.x *= __instance.m_MaxOverloadSpeedMul;
                }
                bool flag5 = __instance.m_TimeInAir < 0.5f;
                if (flag5)
                {
                    __instance.m_WantedSpeed.target = __instance.m_CharacterController.transform.TransformVector(vector2);
                }
                if (flag2)
                {
                    __instance.m_SlideDir = __instance.m_AdditionalSpeed.normalized;
                }
                else if (!flag)
                {
                    __instance.m_SlideDir = Vector3.zero;
                }
                if (flag || (!__instance.m_SlideDir.IsZero() && flag2))
                {
                    Vector3 vector3 = __instance.m_WantedSpeed.target;
                    vector3.y = 0f;
                    Vector3 slideDir = __instance.m_SlideDir;
                    slideDir.y = 0f;
                    if (Vector3.Dot(vector3, slideDir) < 0f)
                    {
                        Vector3 normalized = Vector3.Cross(slideDir, Vector3.up).normalized;
                        vector3 = Vector3.Dot(vector3, normalized) * normalized;
                        __instance.m_WantedSpeed.target.x = vector3.x;
                        __instance.m_WantedSpeed.target.z = vector3.z;
                    }
                }
                if (Time.timeScale > 0f)
                {
                    if (__instance.m_SlideDir.IsZero())
                    {
                        __instance.m_SlidingSpeedSoft.target = Vector3.zero;
                        __instance.m_SlidingSpeedSoft.omega = __instance.m_SlideDeceleration;
                    }
                    else
                    {
                        float proportionalClamp = CJTools.Math.GetProportionalClamp(0f, 1f, __instance.m_SlideAngle, __instance.m_CharacterController.slopeLimit, __instance.m_CharacterController.slopeLimit + 20f);
                        __instance.m_SlidingSpeedSoft.target = __instance.m_SlideDir * __instance.m_SlideMaxSpeed * Mathf.Lerp(0.2f, 1f, proportionalClamp);
                        __instance.m_SlidingSpeedSoft.omega = Mathf.Lerp(__instance.m_SlideAcceleration * 0.1f, __instance.m_SlideAcceleration, proportionalClamp);
                    }
                    if (!flag5)
                    {
                        __instance.m_WantedSpeed.target = __instance.m_WantedSpeed.target - __instance.m_SlidingSpeedSoft;
                    }
                    __instance.m_SlidingSpeedSoft.Update(Time.deltaTime / Time.timeScale);
                    __instance.m_WantedSpeed.target = __instance.m_WantedSpeed.target + __instance.m_SlidingSpeedSoft;
                }
                if (__instance.m_Player.GetUseGravity())
                {
                    __instance.m_WantedSpeed.target = __instance.m_WantedSpeed.target + Physics.gravity * Time.deltaTime;
                }
                else
                {
                    __instance.m_WantedSpeed.current.x = (__instance.m_WantedSpeed.target.x = 0f);
                    __instance.m_WantedSpeed.current.y = (__instance.m_WantedSpeed.target.y = 0f);
                    __instance.m_WantedSpeed.current.z = (__instance.m_WantedSpeed.target.z = 0f);
                }
                if (__instance.m_WantedSpeed.target.y < -10f)
                {
                    __instance.m_WantedSpeed.target.y = -10f;
                }
                Vector3 b = Vector3.zero;
                if (!__instance.m_Player.GetMovesBlocked() && __instance.m_CharacterController.detectCollisions && !__instance.m_SkipCompensation && !FreeHandsLadderController.Get().IsActive())
                {
                    b = -__instance.m_CharacterController.transform.TransformVector(__instance.m_Player.m_CharacterControllerDelta);
                }
                b.y = 0f;
                if ((__instance.m_CollisionFlags & CollisionFlags.Below) == CollisionFlags.None)
                {
                    __instance.m_WantedSpeed.omega = GreenHellMenu.Instance.AccelerationInAir;
                    __instance.m_WantedPos.omega = GreenHellMenu.Instance.AccelerationInAir;
                }
                else if (__instance.m_Dodge)
                {
                    __instance.m_WantedSpeed.omega = __instance.m_DodgeAcceleration;
                    __instance.m_WantedPos.omega = __instance.m_DodgeAcceleration;
                }
                else
                {
                    __instance.m_WantedSpeed.omega = __instance.m_Acceleration;
                    __instance.m_WantedPos.omega = __instance.m_PositionAcceleration;
                }
                if (Time.timeScale > 0f)
                {
                    __instance.m_WantedSpeed.Update(Time.deltaTime / Time.timeScale);
                }
                __instance.m_CurentMoveSpeed = __instance.m_WantedSpeed.current.To2D().magnitude;
                __instance.m_CollisionFlagsAddSpeed = CollisionFlags.None;
                __instance.m_SlideAngle = 0f;
                __instance.m_AdditionalSpeed = Vector3.zero;
                __instance.m_CollisionFlags = __instance.m_CharacterController.Move(__instance.m_Player.m_SpeedMul * __instance.m_WantedSpeed.current * Time.deltaTime, false);
                Vector2.zero.Set(__instance.m_WantedSpeed.current.x, __instance.m_WantedSpeed.current.z);
                if ((__instance.m_CollisionFlags & CollisionFlags.Sides) != CollisionFlags.None)
                {
                    __instance.m_LastSideCollisionTime = Time.time;
                }
                if ((__instance.m_CollisionFlags & CollisionFlags.Below) != CollisionFlags.None && (__instance.m_LastCollisionFlags & CollisionFlags.Below) == CollisionFlags.None)
                {
                    __instance.m_Player.OnLanding(__instance.m_CharacterController.velocity);
                    __instance.m_JumpInProgress = false;
                }
                if ((__instance.m_CollisionFlags & CollisionFlags.Below) != CollisionFlags.None)
                {
                    __instance.m_TimeInAir = 0f;
                }
                else
                {
                    __instance.m_TimeInAir += Time.deltaTime;
                }
                __instance.m_LastCollisionFlags = __instance.m_CollisionFlags;
                __instance.m_SkipCompensation = false;
                __instance.m_WantedPos.current = __instance.transform.position;
                __instance.m_WantedPos.target = __instance.m_CharacterController.transform.position + b;
                if (Time.timeScale > 0f)
                {
                    __instance.m_WantedPos.Update(Time.deltaTime / Time.timeScale);
                }
                __instance.transform.position = __instance.m_WantedPos.current;
                return false;
            }
            return true;
        }

    }
}

