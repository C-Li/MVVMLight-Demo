using System.Threading.Tasks;

namespace MvvmLightDemo.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
    }
}