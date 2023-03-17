using d4160.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static Unity.Services.Lobbies.Models.QueryFilter;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/RuntimeSet")]
    public class RuntimeLobbiesSO : RuntimeSetSOBase<Lobby>
    {
        // TODO: QueryLobbiesOptions
        [Header("Query Options")]
        public int count = 25;
        public int skip = 0;
        public bool randomSampleResults = false;
        public QueryFilterData[] filters;
        public QueryOrderData[] orders;

        public async void ListLobbiesAsync()
        {
            try
            {
                Clear();

                QueryLobbiesOptions options = new() {
                    Count = count,
                    Skip = skip,
                    SampleResults = randomSampleResults,
                    Filters = QueryFilterData.GetFilters(filters),
                    Order = QueryOrderData.GetOrder(orders)
                };

                QueryResponse qResponse = await LobbyService.Instance.QueryLobbiesAsync();

                Debug.Log($"[ListLobbiesAsync] Lobbies Found: {qResponse.Results.Count}");
                for (int i = 0; i < qResponse.Results.Count; i++)
                {
                    Debug.Log($"Id: {qResponse.Results[i].Id}; Name: {qResponse.Results[i].Name}; MaxPlayers: {qResponse.Results[i].MaxPlayers}");
                }
                if (qResponse.Results.Count > 0)
                {
                    AddRange(qResponse.Results);
                }
            }
            catch(LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    [System.Serializable]
    public struct QueryFilterData
    {
        public FieldOptions field;
        public string value;
        public OpOptions op;

        public static List<QueryFilter> GetFilters(QueryFilterData[] filters)
        {
            List<QueryFilter> fs = null;
            if (filters != null && filters.Length > 0)
            {
                fs = new(filters.Length);

                for (int i = 0; i < filters.Length; i++)
                {
                    QueryFilterData f = filters[i];
                    fs.Add(new QueryFilter(f.field, f.value, f.op));
                }
            }

            return fs;
        }
    }

    [System.Serializable]
    public struct QueryOrderData
    {
        public bool asc;
        public QueryOrder.FieldOptions field;

        public static List<QueryOrder> GetOrder(QueryOrderData[] orderData)
        {
            List<QueryOrder> os = null;
            if (orderData != null && orderData.Length > 0)
            {
                os = new(orderData.Length);

                for (int i = 0; i < orderData.Length; i++)
                {
                    QueryOrderData o = orderData[i];
                    os.Add(new QueryOrder(o.asc, o.field));
                }
            }

            return os;
        }
    }
}