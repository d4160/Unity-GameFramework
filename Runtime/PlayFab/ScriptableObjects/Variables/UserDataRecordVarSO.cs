using UnityEngine;
using d4160.Variables;
using PlayFab.ClientModels;

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Variables/UserDataRecord")]
    public class UserDataRecordVarSO : VariableSOBase<UserDataRecord>
    {
        public StringVariableSO valueVar;

        public override UserDataRecord Value { get => base.Value; set { if (valueVar) valueVar.Value = value.Value; base.Value = value; } }
    }
}
