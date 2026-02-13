using UnityEngine;
//using ReadyPlayerMe.Core;
using System;
using System.Threading.Tasks;

public static class AvatarLoadingHelper
{
    private const string FULL_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";
    private const string FULL_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";

    /// <summary>
    /// Load the avatar with url and then transfer mesh data to placeholder
    /// </summary>
    /// <param name="url">The RPM avatar URL or ID</param>
    /// <param name="avatarPlaceholder">The GameObject the loaded avatar will transfer to</param>
    /// <param name="config">The config asset for loading avatar</param>
    /// <param name="onCompleted">Callback after avatar loading completes</param>
    /// <param name="configEyePosition">If we need to configure the eye's positions of avatar placeholder after transfer mesh</param>
    public static void LoadAndTransferAvatar(string url, GameObject avatarPlaceholder, Action onCompleted = null, bool configEyePosition = true) // , AvatarConfig config,
    {
        Transform leftEye = null, rightEye = null;
        if (configEyePosition)
        {
            leftEye = avatarPlaceholder.transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME);
            rightEye = avatarPlaceholder.transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME);
        }

        // var loader = new AvatarObjectLoader();
        // loader.LoadAvatar(url);
        // loader.AvatarConfig = config;
        // loader.OnCompleted += (sender, args) =>
        // {
        //     if (configEyePosition)
        //     {
        //         leftEye.transform.localPosition = args.Avatar.transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME).localPosition;
        //         rightEye.transform.localPosition = args.Avatar.transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME).localPosition;
        //     }

        //     AvatarMeshHelper.TransferMesh(args.Avatar, avatarPlaceholder);
        //     UnityEngine.Object.Destroy(args.Avatar);

        //     onCompleted?.Invoke();
        // };
    }

    /// <summary>
    /// Load the avatar with url and then transfer mesh data to placeholder
    /// </summary>
    /// <param name="url">The RPM avatar URL or ID</param>
    /// <param name="avatarPlaceholder">The GameObject the loaded avatar will transfer to</param>
    /// <param name="config">The config asset for loading avatar</param>
    /// <param name="configEyePosition">If we need to configure the eye's positions of avatar placeholder after transfer mesh</param>
    public static async Task LoadAndTransferAvatarAsync(string url, GameObject avatarPlaceholder, bool configEyePosition = true)
    {
        Transform leftEye = null, rightEye = null;
        if (configEyePosition)
        {
            leftEye = avatarPlaceholder.transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME);
            rightEye = avatarPlaceholder.transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME);
        }

        var loading = true;
        // var loader = new AvatarObjectLoader();
        // loader.LoadAvatar(url);
        // loader.AvatarConfig = config;
        // loader.OnCompleted += (sender, args) =>
        // {
        //     if (configEyePosition)
        //     {
        //         leftEye.transform.localPosition = args.Avatar.transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME).localPosition;
        //         rightEye.transform.localPosition = args.Avatar.transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME).localPosition;
        //     }

        //     AvatarMeshHelper.TransferMesh(args.Avatar, avatarPlaceholder);
        //     UnityEngine.Object.Destroy(args.Avatar);

        //     loading = false;
        // };

        while (loading)
        {
            await Task.Yield();
        }
    }
}
