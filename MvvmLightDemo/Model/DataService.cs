using System.Threading.Tasks;

namespace MvvmLightDemo.Model
{
    public class DataService : IDataService
    {
        public Task<DataItem> GetData()
        {
            // Use this to connect to the actual data service（用来连接实际数据服务）

            // Simulate by returning a DataItem
            var item = new DataItem("Welcome to MVVM Light");
            return Task.FromResult(item);
        }
    }
}