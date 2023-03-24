using Amazon.Runtime;
using UnityEngine;

namespace d4160.AWS.Core
{
    [CreateAssetMenu(menuName = "d4160/AWS/Core/Credentials")]
    public class AWSCredentialsSO : ScriptableObject
    {
        [SerializeField] private string _accessKey;
        [SerializeField] private string _secretKey;

        private BasicAWSCredentials _credentials;

        public BasicAWSCredentials GetCredentials(bool forceCreateNew = false)
        {
            if (_credentials == null || forceCreateNew)
            {
                _credentials = new BasicAWSCredentials(_accessKey, _secretKey);
            }

            return _credentials;
        }
    }

    public static class AWSExtensions
    {
        public static Amazon.RegionEndpoint GetRegionEndpoint(this AWSRegionEndpoint selected)
        {
            switch (selected)
            {
                case AWSRegionEndpoint.AFSouth1:
                    return Amazon.RegionEndpoint.AFSouth1;
                case AWSRegionEndpoint.APEast1:
                    return Amazon.RegionEndpoint.APEast1;
                case AWSRegionEndpoint.APNortheast1:
                    return Amazon.RegionEndpoint.APNortheast1;
                case AWSRegionEndpoint.APNortheast2:
                    return Amazon.RegionEndpoint.APNortheast2;
                case AWSRegionEndpoint.APNortheast3:
                    return Amazon.RegionEndpoint.APNortheast3;
                case AWSRegionEndpoint.APSouth1:
                    return Amazon.RegionEndpoint.APSouth1;
                case AWSRegionEndpoint.APSouth2:
                    return Amazon.RegionEndpoint.APSouth2;
                case AWSRegionEndpoint.APSoutheast1:
                    return Amazon.RegionEndpoint.APSoutheast1;
                case AWSRegionEndpoint.APSoutheast2:
                    return Amazon.RegionEndpoint.APSoutheast2;
                case AWSRegionEndpoint.APSoutheast3:
                    return Amazon.RegionEndpoint.APSoutheast3;
                case AWSRegionEndpoint.APSoutheast4:
                    return Amazon.RegionEndpoint.APSoutheast4;
                case AWSRegionEndpoint.CACentral1:
                    return Amazon.RegionEndpoint.CACentral1;
                case AWSRegionEndpoint.EUCentral1:
                    return Amazon.RegionEndpoint.EUCentral1;
                case AWSRegionEndpoint.EUCentral2:
                    return Amazon.RegionEndpoint.EUCentral2;
                case AWSRegionEndpoint.EUNorth1:
                    return Amazon.RegionEndpoint.EUNorth1;
                case AWSRegionEndpoint.EUSouth1:
                    return Amazon.RegionEndpoint.EUSouth1;
                case AWSRegionEndpoint.EUSouth2:
                    return Amazon.RegionEndpoint.EUSouth2;
                case AWSRegionEndpoint.EUWest1:
                    return Amazon.RegionEndpoint.EUWest1;
                case AWSRegionEndpoint.EUWest2:
                    return Amazon.RegionEndpoint.EUWest2;
                case AWSRegionEndpoint.EUWest3:
                    return Amazon.RegionEndpoint.EUWest3;
                case AWSRegionEndpoint.MECentral1:
                    return Amazon.RegionEndpoint.MECentral1;
                case AWSRegionEndpoint.MESouth1:
                    return Amazon.RegionEndpoint.MESouth1;
                case AWSRegionEndpoint.SAEast1:
                    return Amazon.RegionEndpoint.SAEast1;
                case AWSRegionEndpoint.USEast1:
                    return Amazon.RegionEndpoint.USEast1;
                case AWSRegionEndpoint.USEast2:
                    return Amazon.RegionEndpoint.USEast2;
                case AWSRegionEndpoint.USWest1:
                    return Amazon.RegionEndpoint.USWest1;
                case AWSRegionEndpoint.USWest2:
                    return Amazon.RegionEndpoint.USWest2;
                case AWSRegionEndpoint.CNNorth1:
                    return Amazon.RegionEndpoint.CNNorth1;
                case AWSRegionEndpoint.CNNorthWest1:
                    return Amazon.RegionEndpoint.CNNorthWest1;
                case AWSRegionEndpoint.USGovCloudEast1:
                    return Amazon.RegionEndpoint.USGovCloudEast1;
                case AWSRegionEndpoint.USGovCloudWest1:
                    return Amazon.RegionEndpoint.USGovCloudWest1;
                case AWSRegionEndpoint.USIsoEast1:
                    return Amazon.RegionEndpoint.USIsoEast1;
                case AWSRegionEndpoint.USIsoWest1:
                    return Amazon.RegionEndpoint.USIsoWest1;
                case AWSRegionEndpoint.USIsobEast1:
                    return Amazon.RegionEndpoint.USIsobEast1;
                default:
                    return Amazon.RegionEndpoint.SAEast1;
            }
        }
    }
    public enum AWSRegionEndpoint
    {
        AFSouth1,
        APEast1,
        APNortheast1,
        APNortheast2,
        APNortheast3,
        APSouth1,
        APSouth2,
        APSoutheast1,
        APSoutheast2,
        APSoutheast3,
        APSoutheast4,
        CACentral1,
        EUCentral1,
        EUCentral2,
        EUNorth1,
        EUSouth1,
        EUSouth2,
        EUWest1,
        EUWest2,
        EUWest3,
        MECentral1,
        MESouth1,
        SAEast1,
        USEast1,
        USEast2,
        USWest1,
        USWest2,
        CNNorth1,
        CNNorthWest1,
        USGovCloudEast1,
        USGovCloudWest1,
        USIsoEast1,
        USIsoWest1,
        USIsobEast1
    }
}