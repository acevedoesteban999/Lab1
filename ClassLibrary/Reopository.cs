using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using ClassLibrary;
namespace Repository
{
    public abstract class BaseRepository : IRepository
    {
        protected readonly string ConnectionString;

        protected bool DisposedValue = false;

        protected BaseRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool IsInTransaction { get; protected set; }

        public abstract void BeginTransaction();

        public abstract void PartialCommit();

        public abstract void CommitTransaction();

        public abstract void RollbackTransaction();

        public abstract List<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "") where TEntity : class;
public abstract List<TResult> Get<TEntity, TResult>(Func<TEntity, TResult> selector,
           Expression<Func<TEntity, bool>> filter = null, bool ordered = false) where TEntity : class;
        public abstract Task<List<TEntity>> GetAsync<TEntity>(CancellationTokenSource tokenSource = null, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "") where TEntity : class;
        public abstract Task<List<TResult>> GetAsync<TEntity, TResult>(Func<TEntity, TResult> selector,
           Expression<Func<TEntity, bool>> filter = null, bool ordered = false, CancellationTokenSource tokenSource = null) where TEntity : class;
        public abstract TEntity GetByID<TEntity>(object id) where TEntity : class;

        public abstract void Add<TEntity>(TEntity entity) where TEntity : class;

        public abstract void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        public abstract void Delete<TEntity>(TEntity entityToDelete) where TEntity : class;

        public abstract void DeleteRange<TEntity>(IEnumerable<TEntity> entitiesToDelete) where TEntity : class;

        public abstract void Update<TEntity>(TEntity entityToUpdate) where TEntity : class;

        public abstract void AttachEntity<T>(T entity) where T : class;

        public abstract void Dispose();
    }

    public sealed partial class DBRepository : BaseRepository
    {
        private RepositoryContext _context;

        public DBRepository(string connectionString) : base(connectionString)
        {
            _context = new RepositoryContext(connectionString);

            if (!_context.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                _context.Database.Migrate();

                _context.Database.BeginTransaction();

                _context.SaveChanges();

                _context.Database.CommitTransaction();
            }
            else
            {
                _context.Database.Migrate();
            }

            _context.Dispose();
        }

        #region Transaction Management

        
        public override void BeginTransaction()
        {
            if (!IsInTransaction)
            {
                _context = new RepositoryContext(ConnectionString);
                IsInTransaction = true;
            }
            DisposedValue = false;
        }

        public override void PartialCommit()
        {
            if (!IsInTransaction)
                throw new Exception("The Repository is not in a transaction");
            try
            {
                _context.SaveChanges();
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "An error occurred during the Commit of the transaction.",
                    exception);
            }
        }

        public override void CommitTransaction()
        {
            if (!IsInTransaction)
                throw new Exception("The Repository is not in a transaction");
            try
            {
                _context.SaveChanges();
                _context.Dispose();
                IsInTransaction = false;
            }
            catch (Exception exception)
            {
                _context.Dispose();
                throw new Exception(
                    "An error occurred during the Commit of the transaction.",
                    exception);
            }
        }

        public override void RollbackTransaction()
        {
            if (!IsInTransaction)
                throw new Exception("The Repository is not in a transaction");
            IsInTransaction = false;
            _context.Dispose();
        }

        #endregion

        #region IDisposable Support

        private void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                DisposedValue = true;
            }
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public override List<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "") where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
                query = query.Where(filter);

            foreach (string includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            return orderBy != null
                ? orderBy(query).ToList()
                : query.ToList();
        }

        public override async Task<List<TEntity>> GetAsync<TEntity>(CancellationTokenSource tokenSource = null, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "") where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
                query = query.Where(filter);

            foreach (string includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);
            if (tokenSource == null)
                return orderBy != null
                    ? await orderBy(query).ToListAsync()
                    : await query.ToListAsync();
            return orderBy != null
                ? await orderBy(query).ToListAsync(tokenSource.Token)
                : await query.ToListAsync(tokenSource.Token);
        }

        public override List<TResult> Get<TEntity, TResult>(Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null, bool ordered = false) where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
                query = query.Where(filter);

            IEnumerable<TResult> result = query.Select(selector);

            return ordered
                ? result.ToList()
                : result.OrderBy(x => x).ToList();
        }

        public override async Task<List<TResult>> GetAsync<TEntity, TResult>(Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null, bool ordered = false, CancellationTokenSource tokenSource = null)
            where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
                query = query.Where(filter);

            IQueryable<TResult> result = query.Select(selector).AsQueryable();

            return ordered
                ? await result.ToListAsync(tokenSource.Token)
                : await result.OrderBy(x => x).ToListAsync(tokenSource.Token);
        }

        public override TEntity GetByID<TEntity>(object id) where TEntity : class
        {
            return _context.Set<TEntity>().Find(id);
        }

        public override void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
        }

        public override void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public override void Delete<TEntity>(TEntity entityToDelete) where TEntity : class
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
                _context.Set<TEntity>().Attach(entityToDelete);
            _context.Set<TEntity>().Remove(entityToDelete);
        }

        public override void DeleteRange<TEntity>(IEnumerable<TEntity> entitiesToDelete)
        {
            foreach (var entityToAttach in entitiesToDelete.Where(e => _context.Entry(e).State == EntityState.Detached))
                _context.Set<TEntity>().Attach(entityToAttach);
            _context.Set<TEntity>().RemoveRange(entitiesToDelete);
        }

        public override void Update<TEntity>(TEntity entityToUpdate) where TEntity : class
        {
            _context.Set<TEntity>().Update(entityToUpdate);
        }

        public override void AttachEntity<T>(T entity) where T : class
        {
            _context.Set<T>().Attach(entity);
        }
    }

    public partial class DBRepository : IRepository
    {
        
        public Form CreateForm(string name,FormsType formType,Coordinate coordinate)
        {
            Form form= new Button("", new Coordinate(), 0, 0);
            switch (formType)
            {
                case FormsType.BUTTON:
                    form = new Button(name, coordinate, 15, 15);
                    break;
                case FormsType.RADIOBUTTON:
                    form = new RadioButton(name, coordinate);
                    break;
                case FormsType.CHECTBOX:
                    form = new ChectBox(name, coordinate);
                    break;
                case FormsType.RADIOBUTTONGROUP:
                    form = new RadioButtonGroup(name, coordinate);
                    break;
                case FormsType.BOX:
                    form = new Box(name, coordinate);
                    break;
                case FormsType.FREEBOX:
                    form = new FreeBox(name, coordinate);
                    break;
                case FormsType.LABEL:
                    form = new Label(name,"NullText", coordinate);
                    break;
            
            }
            Add(form);
            return form;
        }
        public void UpdateForm(Form form)
        {
            Update(form);
        }
        
        public void DeleteForm(Form form)
        {
            Delete(form);
        }
       
        public Form FindForm(string name)
        {
            return Get<Form>(w => w._name == name).First();
        }
        public Form GetForm(int id)
        {
            return GetByID<Form>(id);
        }
        public List<Form> GetForm()
        {
            return Get<Form>();
        }
    }

    public interface IRepository : IDisposable
    {
     
        bool IsInTransaction { get; }
        void BeginTransaction();
        void PartialCommit();
        void CommitTransaction();
        void RollbackTransaction();
    }

    public interface IFormRepository : IRepository
    {
        Form CreateForm(string name, FormsType formType, Coordinate coordinate);
        void UpdateForm(Form Form);
        void DeleteForm(Form Form);
        Form FindForm(string name);
        Form GetForm(int id);
        List<Form> GetForms();
    }

    public class RepositoryContext : DbContext
    {
        #region Tables

        public DbSet<Form> Forms { get; set; }

        #endregion

        public RepositoryContext()
        {

        }

        public RepositoryContext(string connectionString) : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>();
            try
            {
                var connectionString = @"Data Source=ESTEBAN-PC\SQLEXPRESS;AttachDBFilename=C:\Users\Esteban\Desktop\Esteban\Programacion\C#\BDForm.mdf;Initial Catalog=BDForm;User ID=sa;Password=qwerty";
                optionsBuilder.UseSqlServer(connectionString);
            }
            catch (Exception)
            {
                //handle errror here.. means DLL has no sattelite configuration file.
            }

            return new RepositoryContext(optionsBuilder.Options);
        }
    }
}
