using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CreateAssets : MonoBehaviour
{
    
        public void spawn()
        {
           CreateAndWaitUntilCompleted("Player");
        }

        private async Task CreateAndWaitUntilCompleted(string label)
        {
            var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
            await Addressables.InstantiateAsync(locations[0]).Task;
        }
    
    
}
