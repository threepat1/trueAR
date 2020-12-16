using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class CreateAddressablesLoader
{
    public static async Task ByLoadedAddress<T>(IList<IResourceLocation> loadedLocations, List<T> createdObjs)
        where T : Object
    {
        foreach (var location in loadedLocations)
        {
            var obj = await Addressables.InstantiateAsync(location).Task as T;
            createdObjs.Add(obj);
        }
    }
}
