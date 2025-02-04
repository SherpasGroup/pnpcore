using PnP.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// Public interface to define a collection of RecycleBinItem objects
    /// </summary>
    [ConcreteType(typeof(RecycleBinItemCollection))]
    public interface IRecycleBinItemCollection : IQueryable<IRecycleBinItem>, IAsyncEnumerable<IRecycleBinItem>, IDataModelCollection<IRecycleBinItem>, IDataModelCollectionLoad<IRecycleBinItem>, IDataModelCollectionDeleteByGuidId, ISupportModules<IRecycleBinItemCollection>
    {
        #region Methods

        #region DeleteAll()
        /// <summary>
        /// Permanently deletes all Recycle Bin items.
        /// </summary>
        public Task DeleteAllAsync();

        /// <summary>
        /// Permanently deletes all Recycle Bin items.
        /// </summary>
        public void DeleteAll();

        /// <summary>
        /// Permanently deletes all Recycle Bin items using a specific batch instance.
        /// </summary>
        public Task DeleteAllBatchAsync(Batch batch);

        /// <summary>
        /// Permanently deletes all Recycle Bin items using a specific batch instance.
        /// </summary>
        public void DeleteAllBatch(Batch batch);

        /// <summary>
        /// Permanently deletes all Recycle Bin items using the current context batch instance.
        /// </summary>
        public Task DeleteAllBatchAsync();

        /// <summary>
        /// Permanently deletes all Recycle Bin items using the current context batch instance.
        /// </summary>
        public void DeleteAllBatch();
        #endregion

        #region DeleteAllSecondStageItems()

        /// <summary>
        /// Permanently deletes second stage Recycle Bin items.
        /// </summary>
        public Task DeleteAllSecondStageItemsAsync();

        /// <summary>
        /// Permanently deletes second stage Recycle Bin items.
        /// </summary>
        public void DeleteAllSecondStageItems();

        /// <summary>
        /// Permanently deletes second stage Recycle Bin items using a specific batch instance.
        /// </summary>
        public Task DeleteAllSecondStageItemsBatchAsync(Batch batch);

        /// <summary>
        /// Permanently deletes second stage Recycle Bin items using a specific batch instance.
        /// </summary>
        public void DeleteAllSecondStageItemsBatch(Batch batch);

        /// <summary>
        /// Permanently deletes second stage Recycle Bin items using the current context batch instance.
        /// </summary>
        public Task DeleteAllSecondStageItemsBatchAsync();

        /// <summary>
        /// Permanently deletes all Recycle Bin items using the current context batch instance.
        /// </summary>
        public void DeleteAllSecondStageItemsBatch();
        #endregion

        #region MoveAllToSecondStage()
        /// <summary>
        /// Move all Recycle Bin items to second stage.
        /// </summary>
        public Task MoveAllToSecondStageAsync();

        /// <summary>
        /// Move all Recycle Bin items to second stage.
        /// </summary>
        public void MoveAllToSecondStage();

        /// <summary>
        /// Move all Recycle Bin items to second stage using a specific batch instance.
        /// </summary>
        public Task MoveAllToSecondStageBatchAsync(Batch batch);

        /// <summary>
        /// Move all Recycle Bin items to second stage using a specific batch instance.
        /// </summary>
        public void MoveAllToSecondStageBatch(Batch batch);

        /// <summary>
        /// Move all Recycle Bin items to second stage using the current context batch instance.
        /// </summary>
        public Task MoveAllToSecondStageBatchAsync();

        /// <summary>
        /// Move all Recycle Bin items to second stage using the current context batch instance.
        /// </summary>
        public void MoveAllToSecondStageBatch();
        #endregion

        #region RestoreAll()
        /// <summary>
        /// Restores all Recycle Bin items to their original locations.
        /// </summary>
        public Task RestoreAllAsync();

        /// <summary>
        /// Restores all Recycle Bin items to their original locations.
        /// </summary>
        public void RestoreAll();

        /// <summary>
        /// Restores all Recycle Bin items to their original locations using a specific batch instance.
        /// </summary>
        public Task RestoreAllBatchAsync(Batch batch);

        /// <summary>
        /// Restores all Recycle Bin items to their original locations using a specific batch instance.
        /// </summary>
        public void RestoreAllBatch(Batch batch);

        /// <summary>
        /// Restores all Recycle Bin items to their original locations using the current context batch instance.
        /// </summary>
        public Task RestoreAllBatchAsync();

        /// <summary>
        /// Restores all Recycle Bin items to their original locations using the current context batch instance.
        /// </summary>
        public void RestoreAllBatch();
        #endregion

        #region GetById methods

        /// <summary>
        /// Method to select a recycle bin item (<c>IRecycleBinItem</c>) by id
        /// </summary>
        /// <param name="id">The Id to search for</param>
        /// <param name="selectors">The expressions declaring the fields to select</param>
        /// <returns>The resulting recycle bin item instance, if any</returns>
        public IRecycleBinItem GetById(Guid id, params Expression<Func<IRecycleBinItem, object>>[] selectors);

        /// <summary>
        /// Method to select a recycle bin item (<c>IRecycleBinItem</c>) by id asynchronously
        /// </summary>
        /// <param name="id">The Id to search for</param>
        /// <param name="selectors">The expressions declaring the fields to select</param>
        /// <returns>The resulting recycle bin item instance, if any</returns>
        public Task<IRecycleBinItem> GetByIdAsync(Guid id, params Expression<Func<IRecycleBinItem, object>>[] selectors);

        #endregion

        #endregion
    }
}