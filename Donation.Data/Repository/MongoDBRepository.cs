using Donation.Data.Context;
using Donation.Data.Extensions;
using Donation.Domain.Collections;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Donation.Data.Repository
{
    public class MongoDBRepository<TEntity> : IMongoDBRepository<TEntity> where TEntity : class, BaseCollection
    {
        private readonly IMongoCollection<TEntity> dbSet = null;
        private readonly DonationContext _dataContext;
        public MongoDBRepository(DonationContext dataContext)
        {
            _dataContext = dataContext;
            dbSet = _dataContext.GetCollection<TEntity>();
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = System.DateTime.UtcNow;
            entity.ModifiedDate = System.DateTime.UtcNow;
            await dbSet.InsertOneAsync(entity);
            return entity;
        }

        public bool AddAll(List<TEntity> entity)
        {
            bool status = false;
            try
            {
                entity.ForEach(x => x.CreatedDate = DateTime.UtcNow);
                entity.ForEach(x => x.ModifiedDate = DateTime.UtcNow);
                dbSet.InsertManyAsync(entity);
                status = true;
            }
            catch (Exception ex)
            {
                status = true;
            }
            return status;
        }

        public async Task<TEntity> Delete(FilterDefinition<TEntity> entity, TEntity model)
        {
            await dbSet.ReplaceOneAsync(entity, model);
            return model;
        }

        public async Task<TEntity> DeleteOne(FilterDefinition<TEntity> entity, TEntity model)
        {
            await dbSet.DeleteOneAsync(entity);
            return model;
        }

        public bool Exist(Expression<Func<TEntity, bool>> whereCondition, bool WithDeletedObjects = false)
        {
            return GetQuery(whereCondition, WithDeletedObjects).Any();
        }

        public IQueryable<TEntity> GetAll(bool WithDeletedObjects = false)
        {
            return GetQuery(WithDeletedObjects: WithDeletedObjects);
        }

        public IQueryable<TEntity> GetById(Expression<Func<TEntity, bool>> whereCondition, bool WithDeletedObjects = false)
        {
            return GetQuery(filter: whereCondition, WithDeletedObjects: WithDeletedObjects);
        }

        public IMongoCollection<TEntity> GetCollection()
        {
            return dbSet;
        }

        public async Task<ReplaceOneResult> Update(FilterDefinition<TEntity> filter, TEntity model)
        {
            var updateRes = await dbSet.ReplaceOneAsync(filter, model);
            return updateRes;
        }

        private IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null, bool WithDeletedObjects = false)
        {
            filter = filter == null ? x => true : filter;
            var collection = dbSet;
            IQueryable<TEntity> query = collection.AsQueryable();
            query = query.Where(filter).ApplyDefaultFilters(WithDeletedObjects);
            return query;
        }
    }
}
