﻿using AdminLTE.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace AdminLTE.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly DbContext Context;
        private readonly IPrincipal User;

        public Repository(DbContext context, IPrincipal user)
        {
            Context = context;
            User = user;
        }

        public TEntity Add(TEntity entity)
        {
            if (typeof(TEntity).GetInterfaces().Any(c => c == typeof(ITracingEntity)))
            {
                var tEntity = (ITracingEntity)entity;
                tEntity.CreatedDate = DateTime.Now;
                tEntity.CreatedUser = User.Identity.Name;
            }
            return Context.Set<TEntity>().Add(entity);
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            if (typeof(TEntity).GetInterfaces().Any(c => c == typeof(ITracingEntity)))
            {
                foreach (var entity in entities)
                {
                    var tEntity = (ITracingEntity)entity;
                    tEntity.CreatedDate = DateTime.Now;
                    tEntity.CreatedUser = User.Identity.Name;
                }
            }
            return Context.Set<TEntity>().AddRange(entities);
        }

        public TEntity Update(TEntity entity)
        {
            try
            {
                if (typeof(TEntity).GetInterfaces().Any(c => c == typeof(ITracingEntity)))
                {
                    var tEntity = (ITracingEntity)entity;
                    tEntity.ModifiedDate = DateTime.Now;
                    tEntity.ModifiedUser = User.Identity.Name;
                }
                Context.Set<TEntity>().Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                return entity;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public TEntity Get(params object[] keyValues)
        {
            return Context.Set<TEntity>().Find(keyValues);
        }

        public IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>();
        }

        public TEntity Remove(TEntity entity)
        {
            return Context.Set<TEntity>().Remove(entity);
        }

        public IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
        {
            return Context.Set<TEntity>().RemoveRange(entities);
        }
    }
}
