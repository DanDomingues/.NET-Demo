using Demo.Models;
using Demo.Utility;
using Demo.DataAccess.IRepository;

namespace Demo.DataAccess
{
    public class DataUtility
    {
        public static TOut MoveInList<TModel, TKey, TOut>(
            TModel? element,
            List<TModel> list,
            Func<int, int> getNewIndex,
            Func<TOut> onSuccess,
            Func<string, TOut> onFail,
            Action<TModel[]> onNewList) where TKey : IComparable where TModel : ModelBase
        {
            return GenericUtility.MoveInList(
                element, 
                list, 
                getNewIndex, 
                compare: (c1, c2) => c1.Id.Equals(c2.Id),
                getKey: c => c.Id, 
                onSuccess, 
                onFail, 
                onNewList);
        }

        public static TOut MoveInList<TModel, TKey, TOut>(
            IUnitOfWork unitOfWork,
            IRepository<TModel> repo,
            TModel? element,
            List<TModel> list,
            Func<int, int> getNewIndex,
            Func<TOut> onSuccess,
            Func<string, TOut> onFail,
            Action<TModel[]>? onNewList = null) 
            where TKey : IComparable 
            where TModel : ModelBase, IOrderableModel
        {
            void OnNewList(TModel[] categories)
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    if(categories[i].DisplayOrder != i)
                    {
                        categories[i].DisplayOrder = i;
                        repo.Update(categories[i]);
                    }
                }
                unitOfWork.Save();
                onNewList?.Invoke(categories);
            }

            return MoveInList<TModel, TKey, TOut>(
                element, 
                list, 
                getNewIndex, 
                onSuccess, 
                onFail, 
                OnNewList);
        }
    }
}