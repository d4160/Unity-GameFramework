using d4160.MonoBehaviourData;

namespace d4160.SceneManagement
{
    public class SceneInfoBehaviour : MonoBehaviourData<SceneInfoSO>
    {
        public void ContinueLoadAsync()
        {
            _data.ContinueLoadAsync();
        }
    }
}