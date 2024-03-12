using d4160.Collections;
using UnityEngine;

namespace d4160.UGS.Core
{
    [CreateAssetMenu(menuName = "d4160/UGS/Core/Data/Project")]
    public class ProjectDataSO : ScriptableObject
    {
        [SerializeField] private string _projectId;
        [SerializeField] private string[] _environmentsId;

        public string ProjectId => _projectId;

        /// <summary>
        /// The first EnvironmentId of the list.
        /// </summary>
        /// <returns>Empty if there is any EnvironmentId</returns>
        public string EnvironmentId => GetEnvironmentId(0);

        public string GetEnvironmentId(int index)
        {
            if (_environmentsId.IsValidIndex(index))
            {
                return _environmentsId[index];
            }

            return string.Empty;
        }
    }
}
