using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Exceptions;
using Core.ElasticSearch.Tests.Models;
using Core.ElasticSearch.Tests.Projections;
using Elasticsearch.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch.Tests
{
    [TestClass]
    public class BaseServiceTest : BaseTest
    {
        [TestMethod]
        public void AddObjectWithoutParentAndRelated()
        {
            var category = new NewCategory { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category);

            Assert.IsNotNull(category.Id);
        }

		// TODO: Тест не компилируется за счет введения BaseNewEntity
        //[TestMethod]
        //public void AddObjectWithInvalidRelatedAndWithoutParent()
        //{
        //    var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
        //    var childCategory = new NewCategory() { Name = "Child Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
        //    Assert.ThrowsException<UnexpectedElasticsearchClientException>(() =>
        //    {
        //        _repository.Insert(childCategory);
        //    });
        //}

        [TestMethod]
        public void AddObjectWithValidRelatedAndWithoutParent()
        {
            var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			Assert.IsNotNull(parent);
			Assert.IsNotNull(parent.Id);
			Assert.AreEqual(parent.Id, parentCategory.Id);
			Assert.AreEqual(parent.Version, 1);
			var childCategory = new NewCategory() { Name = "Child Category", Top = parent, CreatedOnUtc = DateTime.UtcNow };
            Assert.IsTrue(_repository.Insert(childCategory));
            Assert.IsNotNull(childCategory.Id);
        }

	    [TestMethod]
	    public void AddObjectWithCustomId()
	    {
		    var category1 = new NewCategoryWithId("my_id1") { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
		    var c1 = _repository.InsertWithIdAndVersion<NewCategoryWithId, Category>(category1);
		    Assert.IsNotNull(c1);
		    Assert.AreEqual(c1.Id, "my_id1");
		    Assert.AreEqual(c1.Id, category1.Id);
			Assert.AreEqual(c1.Version, 1);

		    var category2 = new NewCategoryWithId("my_id2") { Name = "Child Category", CreatedOnUtc = DateTime.UtcNow };
		    var c2 = _repository.InsertWithIdAndVersion<NewCategoryWithId, Category>(category2);
		    Assert.IsNotNull(c2);
		    Assert.AreEqual(c2.Id, "my_id2");
		    Assert.AreEqual(c2.Id, category2.Id);
		    Assert.AreEqual(c2.Version, 1);
	    }

	    [TestMethod]
	    public void AddObjectWithCustomIdWithError()
	    {
		    var category1 = new NewCategoryWithId("my_id1") { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
		    var c1 = _repository.InsertWithIdAndVersion<NewCategoryWithId, Category>(category1);
		    Assert.IsNotNull(c1);
		    Assert.AreEqual(c1.Id, "my_id1");
		    Assert.AreEqual(c1.Id, category1.Id);
		    Assert.AreEqual(c1.Version, 1);

		    var category2 = new NewCategoryWithId("my_id1") { Name = "Child Category", CreatedOnUtc = DateTime.UtcNow };
		    Assert.ThrowsException<VersionException>(()=>_repository.InsertWithIdAndVersion<NewCategoryWithId, Category>(category2));
	    }

		//TODO: Такое использование неприемлено и не компилируется
		//[TestMethod]
		//public void AddObjectWithInvalidParentAndWithoutRelated()
		//{
		//    var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
		//    var product = new Product() { Name = "Product", Parent = category };
		//    Assert.ThrowsException<QueryException>(() => _repository.Insert<Product, Category>(product, true));
		//}
		//TODO: Воторой пример: Такое использование неприемлено и не компилируется
		//[TestMethod]
		//public void AddObjectWithInvalidParentAndWithoutRelated()
		//{
		//	var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
		//	var product = new NewProduct(category) { Name = "Product" };
		//	Assert.ThrowsException<QueryException>(() => _repository.Insert<Product, Category>(product, true));
		//}

		[TestMethod]
        public void AddObjectWithValidParentAndWithoutRelated()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            var parent =  _repository.Insert<NewCategory, CategoryProjection>(category);
            var product = new NewProduct(parent) { Name = "Product", FullName = new FullName() { Name = "Product", Category = category.Name } };
	        _repository.ClearCache();
            _repository.InsertWithParent<NewProduct, CategoryProjection>(product);
            Assert.IsNotNull(product.Id);
            Assert.IsNotNull(product.Parent);
            Assert.AreEqual(product.Parent.Id, category.Id);
            Assert.AreEqual(product.Parent.Name, category.Name);
        }

        [TestMethod]
        public void GetObjectByIdWithoutAutoLoadAndWithoutParent()
        {
            var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            var parent =  _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
            var category = new NewCategory() { Name = "Category", Top = parent, CreatedOnUtc = DateTime.UtcNow };

	        Assert.IsTrue(_repository.Insert(category));
	        _repository.ClearCache();
			var loadCategory = _repository.GetWithVersion<Category>(category.Id, false);

            Assert.IsNotNull(loadCategory);
            Assert.IsNotNull(loadCategory.Top);
            Assert.AreEqual(loadCategory.Top.Id, parentCategory.Id);
            Assert.IsNull(loadCategory.Top.Name);
            Assert.AreNotEqual(loadCategory, category);
            Assert.AreEqual(loadCategory.Name, category.Name);
            Assert.AreEqual(loadCategory.Id, category.Id);
        }

        [TestMethod]
        public void GetProjectionObjectByIdWithoutAutoLoadAndWithoutParent()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };

            Assert.IsTrue(_repository.Insert(category));

	        _repository.ClearCache();
            var loadCategory = _repository.Get<CategoryProjection>(category.Id, false);

            Assert.IsNotNull(loadCategory);
            Assert.IsNull(loadCategory.Top);
            Assert.AreNotEqual(loadCategory, category);
            Assert.AreEqual(loadCategory.Name, category.Name);
            Assert.AreEqual(loadCategory.Id, category.Id);
        }

	    [TestMethod]
	    public void GetProjectionObjectByIdWithAutoLoad()
	    {
		    var top = new NewCategory() {Name = "Top"};
		    var  t = _repository.InsertWithVersion<NewCategory, CategoryJoin>(top);

			var category = new NewCategory() { Name = "Category", Top = t, CreatedOnUtc = DateTime.UtcNow };

		    Assert.IsTrue(_repository.Insert(category));

		    _repository.ClearCache();
		    var loadCategory = _repository.Get<CategoryProjection>(category.Id, true);

		    Assert.IsNotNull(loadCategory);
		    Assert.IsNotNull(loadCategory.Top);
		    Assert.AreEqual(loadCategory.Top.Name, t.Name);
		    Assert.AreEqual(loadCategory.Top.Id, t.Id);
		    Assert.AreNotEqual(loadCategory, category);
		    Assert.AreEqual(loadCategory.Name, category.Name);
		    Assert.AreEqual(loadCategory.Id, category.Id);
	    }

		[TestMethod]
        public void GetObjectByIdWithoutAutoLoadAndWithParent()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            var parent = _repository.Insert<NewCategory, CategoryProjection>(category);
            var product = new NewProduct(parent) { Name = "Product", FullName = new FullName() { Name = "Product", Category = category.Name } };

			Assert.IsTrue(_repository.InsertWithParent<NewProduct, CategoryProjection>(product));

	        _repository.ClearCache();
            var loadProduct = _repository.GetWithVersion<Product, CategoryProjection>(product.Id, category.Id, false);

            Assert.IsNotNull(loadProduct);
            Assert.IsNotNull(loadProduct.Parent);
            Assert.AreEqual(loadProduct.Parent.Id, category.Id);
            Assert.IsNull(loadProduct.Parent.Name);
            Assert.IsNotNull(loadProduct.FullName);
            Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
        }

        [TestMethod]
        public void GetProjectionObjectByIdWithoutAutoLoadAndWithParent()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
	        var parent = _repository.Insert<NewCategory, CategoryProjection>(category);
			var product = new NewProduct(parent) { Name = "Product", FullName = new FullName() { Name = "Product", Category = category.Name } };
            _repository.InsertWithParent<NewProduct, CategoryProjection>(product);

	        _repository.ClearCache();
            var loadProduct = _repository.GetWithVersion<ProductProjection, CategoryProjection>(product.Id, category.Id, false);

            Assert.IsNotNull(loadProduct);
			Assert.AreEqual(loadProduct.Version, 1);
            Assert.IsNotNull(loadProduct.Parent);
            Assert.AreEqual(loadProduct.Parent.Id, category.Id);
            Assert.AreEqual(loadProduct.Parent.Name, null);
            Assert.IsNull(loadProduct.Parent.Name);
			Assert.IsNotNull(loadProduct.FullName);
            Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
        }

        [TestMethod]
        public void GetObjectByIdWithAutoLoadAndWithoutParent()
        {
            var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
	        var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
            var category = new NewCategory() { Name = "Category", Top = parent };

            _repository.Insert(category);

	        _repository.ClearCache();
            var loadCategory = _repository.GetWithVersion<Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.IsNotNull(loadCategory.Top);
            Assert.AreEqual(loadCategory.Top.Id, parentCategory.Id);
            Assert.AreEqual(loadCategory.Top.Name, parentCategory.Name);
            Assert.AreNotEqual(loadCategory, category);
            Assert.AreEqual(loadCategory.Name, category.Name);
            Assert.AreEqual(loadCategory.Id, category.Id);
        }

        [TestMethod]
        public void GetProjectionObjectByIdWithAutoLoadAndWithoutParent()
        {
            var category = new NewCategory() { Name = "Category" };

            _repository.Insert(category);

	        _repository.ClearCache();
            var loadCategory = _repository.Get<CategoryProjection>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.IsNull(loadCategory.Top);
            Assert.AreNotEqual(loadCategory, category);
            Assert.AreEqual(loadCategory.Name, category.Name);
            Assert.AreEqual(loadCategory.Id, category.Id);
        }

        [TestMethod]
        public void GetObjectByIdWithAutoLoadAndWithParent()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
	        var parent = _repository.Insert<NewCategory, CategoryProjection>(category);
            var product = new NewProduct(parent) { Name = "Product", FullName = new FullName() { Name = "Product", Category = category.Name } };

            Assert.IsTrue(_repository.InsertWithParent<NewProduct, CategoryProjection>(product));

	        _repository.ClearCache();
            var loadProduct = _repository.GetWithVersion<ProductProjection, CategoryProjection>(product.Id, category.Id, true);

            Assert.IsNotNull(loadProduct);
            Assert.IsNotNull(loadProduct.Parent);
            Assert.IsNotNull(loadProduct.FullName);
            Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
            Assert.AreEqual(loadProduct.Parent.Id, product.Parent.Id);
            Assert.AreEqual(loadProduct.Parent.Name, product.Parent.Name);
        }

        [TestMethod]
        public void GetProjectionObjectByIdWithAutoLoadAndWithParent()
        {
	        var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
	        var parent = _repository.Insert<NewCategory, CategoryProjection>(category);
	        var product = new NewProduct(parent) { Name = "Product", FullName = new FullName() { Name = "Product", Category = category.Name } };

	        Assert.IsTrue(_repository.InsertWithParent<NewProduct, CategoryProjection>(product));

	        _repository.ClearCache();
			var loadProduct = _repository.GetWithVersion<ProductProjection, CategoryProjection>(product.Id, category.Id, true);

            Assert.IsNotNull(loadProduct);
            Assert.IsNotNull(loadProduct.Parent);
            Assert.AreEqual(loadProduct.Parent.Name, parent.Name);
            Assert.IsNotNull(loadProduct.FullName);
			Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
        }

		[TestMethod]
        public void UpdateObjectSimply()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            var c = _repository.InsertWithVersion<NewCategory, Category>(category);
            c.Name = "New Category";
            _repository.UpdateWithVersion(c);

	        _repository.ClearCache();
            var loadCategory = _repository.GetWithVersion<Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.AreEqual(loadCategory.Version, c.Version);
            Assert.AreEqual(loadCategory.Version, 2);
			Assert.AreEqual(loadCategory.Name, "New Category");
        }

        //[TestMethod]
        //public void UpdateObjectSimplyWithInvalidObjectId()
        //{
        //    var category = new Category() { Name = "Category" };
        //    category.Name = "New Category";
        //    category.Id = "NewId";
        // category.Version = 2;
        //    Assert.ThrowsException<QueryException>(() => _repository.Update(category, true));
        //}

        // Тест больше не актуален, т.к. нельзя никак получить проекции и управлять номером версии в ней. А так же нельзя никак обновить проекцию в базе созданную из вне движка.
        //[TestMethod]
        //      public void UpdateObjectSimplyWithAnotherVersion()
        //      {
        //          var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
        //          _repository.Insert(category, true);
        //          category.Name = "New1 Category";
        //          _repository.Update(category, true);
        //          category.Version--;
        //          category.Name = "New2 Category";
        //          Assert.ThrowsException<VersionException>(() => _repository.Update(category, true));

        //          var loadCategory = _repository.Get<Category>(category.Id, true);

        //          Assert.IsNotNull(loadCategory);
        //          Assert.AreEqual(loadCategory.Name, "New1 Category");
        //      }

        [TestMethod]
        public void UpdateObjectByQueryFuncSet()
        {
            var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category);
            _repository.Update<Category>(q => q.Ids(x => x.Values(category.Id)), u => u.Set(x => x.Name, "New Category"));

	        _repository.ClearCache();
            var loadCategory = _repository.GetWithVersion<Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.AreEqual(loadCategory.Name, "New Category");
            Assert.AreEqual(loadCategory.Version, 2);
        }

		[TestMethod]
		public void UpdateObjectByQueryFuncUnset()
		{
			var category1 = new NewCategory() { Name = "Category 1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category 2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			Assert.AreEqual(2, _repository.Update<Category>(q => q.Ids(x => x.Values(category1.Id, category2.Id)), u => u.Unset(x => x.Name)));

	        _repository.ClearCache();
			var loadCategory1 = _repository.GetWithVersion<Category>(category1.Id, true);
			var loadCategory2 = _repository.GetWithVersion<Category>(category2.Id, true);

			Assert.IsNotNull(loadCategory1);
			Assert.IsNull(loadCategory1.Name);
			Assert.AreEqual(2, loadCategory1.Version);
			Assert.IsNotNull(loadCategory2);
			Assert.IsNull(loadCategory2.Name);
			Assert.AreEqual(2, loadCategory2.Version);
		}

		[TestMethod]
		public void RemoveObject()
		{
			var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
			var c = _repository.InsertWithVersion<NewCategory, Category>(category);
			_repository.RemoveWithVersion(c);

	        _repository.ClearCache();
			var loadCategory = _repository.GetWithVersion<Category>(category.Id, true);
			Assert.IsNull(loadCategory);
		}

		// Ограничил такое поведение на уровне компиляции (Id - internal set)
		//[TestMethod]
		//public void RemoveObjectByInvalidId()
		//{
		//    var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
		//    category.Id = "NewId";
		// category.Version = 1;
		//    Assert.ThrowsException<VersionException>(() => _repository.Remove(category));
		//}

		[TestMethod]
		public void RemoveObjectByQuery()
		{
			var category = new NewCategory() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category);
			_repository.Remove<Category>(q => q.Ids(x => x.Values(category.Id)));

			var loadCategory = _repository.GetWithVersion<Category>(category.Id, true);
			Assert.IsNull(loadCategory);
		}

		[TestMethod]
		public void SearchSimpleCategory()
		{
			var category1 = new NewCategory() { Name = "Test Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Test Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);
			var category3 = new NewCategory() { Name = "Test Category3", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category3);
			var category4 = new NewCategory() { Name = "Test Category4", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category4);
			var category5 = new NewCategory() { Name = "Test Category", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category5);

			var categories = _repository.Filter<Category, Category>(q => q.Match(x => x.Field(c => c.Name).Query("category")));

			Assert.AreEqual(categories.Count, 1);
			Assert.IsTrue(categories.Any(c => c.Name.Equals("Test Category")));
		}

		[TestMethod]
		public void SearchSimpleCategoryProjection()
		{
			var category1 = new NewCategory() { Name = "Test Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Test Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);
			var category3 = new NewCategory() { Name = "Test Category3", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category3);
			var category4 = new NewCategory() { Name = "Test Category4", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category4);
			var category5 = new NewCategory() { Name = "Test Category", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category5);

			var categories = _repository.Filter<Category, CategoryProjection>(q => q.Match(x => x.Field(c => c.Name).Query("category")));

			Assert.AreEqual(categories.Count, 1);
			Assert.IsTrue(categories.Any(c => c.Name.Equals("Test Category")));
		}

		[TestMethod]
		public void SearchCategoryByRelatedWithoutLimitationAndWithoutLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

	        _repository.ClearCache();
			var childCategories = _repository.Filter<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 0, 0, false);

			Assert.AreEqual(childCategories.Count, 3);
			Assert.IsFalse(childCategories.Any(c => c.Name.Equals("Category1")));
			Assert.IsFalse(childCategories.Any(c => c.Name.Equals("Category2")));
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
			Assert.AreEqual(childCategories.FirstOrDefault().Name, "Child Category3");
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Name, null);
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByRelatedWithLimitationAndWithoutLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			_repository.ClearCache();

			var childCategories = _repository.Filter<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Ascending(c => c.CreatedOnUtc), 1, 1, false);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Id, parentCategory.Id);
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Name, null);
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByRelatedWithLimitationAndWithLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			_repository.ClearCache();

			var childCategories = _repository.Filter<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Ascending(c => c.CreatedOnUtc), 1, 1, true);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
			Assert.IsNotNull(childCategories.FirstOrDefault().Top);
		}

		[TestMethod]
		public void SearchCategoryByParentWithLambdaAndWithoutLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			_repository.ClearCache();

			var childCategories = _repository.Search<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 1, 2, false);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Id, parentCategory.Id);
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByParentWithLambdaAndWithLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			_repository.ClearCache();
			var childCategories = _repository.Search<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 1, 2, true);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
			Assert.IsNotNull(childCategories.FirstOrDefault().Top);
		}


		[TestMethod]
		public void SearchCategoryByParentWithPagingAndWithoutLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			_repository.ClearCache();

			var childCategories = _repository.FilterPager<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 0, 1, sort => sort.Descending(c => c.CreatedOnUtc), false);

			Assert.AreEqual(childCategories.Total, 3);
			Assert.AreEqual(childCategories.Limit, 1);
			Assert.AreEqual(childCategories.Page, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Name, null);
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByParentWithPagingAndWithLoad()
		{
			var parentCategory = new NewCategory() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
			var parent = _repository.InsertWithVersion<NewCategory, CategoryJoin>(parentCategory);
			var childCategory1 = new NewCategory() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory1);
			var childCategory2 = new NewCategory() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory2);
			var childCategory3 = new NewCategory() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parent };
			_repository.Insert(childCategory3);
			var category1 = new NewCategory() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category1);
			var category2 = new NewCategory() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
			_repository.Insert(category2);

			_repository.ClearCache();

			var childCategories = _repository.FilterPager<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 1, 2, sort => sort.Descending(c => c.CreatedOnUtc), true);

			Assert.AreEqual(childCategories.Total, 3);
			Assert.AreEqual(childCategories.Limit, 2);
			Assert.AreEqual(childCategories.Page, 1);
			Assert.AreEqual(childCategories.Count, 2);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
			Assert.IsNotNull(childCategories.FirstOrDefault().Top);
			Assert.IsNotNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void InsertIntoAnotherIndex()
		{
			var producer = new NewProducer { Name = "Producer1" };
			_repository.Insert(producer);

			var loaded = _repository.GetWithVersion<Producer>(producer.Id);

			Assert.AreEqual(loaded.Name, producer.Name);
		}

		[TestMethod]
		public void InsertIntoAnotherIndexWithJoin()
		{
			var producer = new NewProducer { Name = "Producer1" };
			var p = _repository.Insert<NewProducer, ProducerProjection>(producer);

			var category = new NewCategory { Name = "Category1" };
			var c = _repository.Insert<NewCategory, CategoryProjection>(category);

			var product = new NewProduct(c) { Name = "Product1", Producer = p };
			_repository.InsertWithParent<NewProduct, CategoryProjection>(product);

			_repository.ClearCache();
			var loaded = _repository.GetWithVersion<Product, CategoryProjection>(product.Id, category.Id, true);

			Assert.AreEqual(loaded.Name, product.Name);
			Assert.AreEqual(loaded.Producer.Id, product.Producer.Id);
			Assert.AreEqual(loaded.Producer.Name, product.Producer.Name);
			Assert.AreEqual(loaded.Parent.Id, product.Parent.Id);
			Assert.AreEqual(loaded.Parent.Name, product.Parent.Name);
		}

		[TestMethod]
		public void UpdateProjection()
		{
			// 1. Добавить новую проекцию с 1 private set полем и несколькими с public set
			// 2. Вставить в базу полную проекцию включая поля, которые не указаны в новой проекции
			// 3. Достать по Id новую проекцию (у проекции для этого должен быть IGetProjection)
			// 4. Обновить
			// 5. Достать полную проекцию и проверить, что обновляемые поля обновлены, а другие остались нетронутыми

			var user = new NewUser { Login = "user1", Email = "user1@user1.ru", Password = "123", Salt = "111" };
			_repository.Insert(user);

			var loaded = _repository.GetWithVersion<UserUpdateProjection>(user.Id, true);
			loaded.Email = "new@user1.ru";
			loaded.Password = "newPass";

			Assert.IsTrue(_repository.UpdateWithVersion(loaded));

			var loadedFullUser = _repository.Get<User>(user.Id, true);

			Assert.AreEqual("user1", loadedFullUser.Login);
			Assert.AreEqual("new@user1.ru", loadedFullUser.Email);
			Assert.AreEqual("newPass", loadedFullUser.Password);
			Assert.AreEqual("111", loadedFullUser.Salt);
		}

		//     [TestMethod]
		//     public void HtmlStripTest()
		//     {

		//     }

		[TestMethod]
		public void AutocompleteTest()
		{
			var category = new NewCategory { Name = "Category1" };
			var c = _repository.Insert<NewCategory, CategoryProjection>(category);

			var product = new NewProduct(c) { Name = "Product1", Title = "ProductA" };
			_repository.InsertWithParent<NewProduct, CategoryProjection>(product);

			product = new NewProduct(c) { Name = "Product2", Title = "ProductA1" };
			_repository.InsertWithParent<NewProduct, CategoryProjection>(product);

			product = new NewProduct(c) { Name = "Product3", Title = "ProductB" };
			_repository.InsertWithParent<NewProduct, CategoryProjection>(product);

			var products = _repository.CompletionSuggest<Product, Product>(s => s.Field(p => p.Title).Prefix("pr"));

			Assert.IsTrue(products.Any());

			products = _repository.CompletionSuggest<Product, Product>(s => s.Field(p => p.Title).Prefix("productA"));

			Assert.IsTrue(products.Any());
			Assert.AreEqual(products.Count, 2);
			// Есть уже анализатор, но лучше покурить это: https://www.red-gate.com/simple-talk/dotnet/net-development/how-to-build-a-search-page-with-elasticsearch-and-net/
			// NGrams: https://qbox.io/blog/multi-field-partial-word-autocomplete-in-elasticsearch-using-ngrams
		}

		//[TestMethod]
		//   public void NestedTest()
		//   {
		//	var category = new NewCategory { Name = "Category1" };
		//    var c = _repository.InsertWithVersion<NewCategory, Category>(category);

		//    var product1 = new NewProduct(c) { Name = "Product1", Title = "ProductA", FullName = new FullName(){Category = c.Name, Name = "Product1", Producer = "Producer1"}};
		//    _repository.InsertWithParent<NewProduct, Category>(product1);
		//    var product2 = new NewProduct(c) { Name = "Product2", Title = "ProductB", FullName = new FullName() { Category = c.Name, Name = "Product2", Producer = "Producer1" } };
		//    _repository.InsertWithParent<NewProduct, Category>(product2);

		//	var result = _repository.FilterNested<Product, FullNameNested>(q => q.Bool(b => b.Filter(f => f.Term(t => t.Field(p => p.Name).Value("Product1")))), p=>p.FullName);
		//   }


		private async Task<long> Load(string url)
	    {
		    var request = WebRequest.CreateHttp(url);
			var sw = new Stopwatch();
		    sw.Start();
		    using (var response = await request.GetResponseAsync())
		    {
			    sw.Stop();
			    return sw.ElapsedMilliseconds;
		    }
	    }

	  //  [TestMethod]
	  //  public void PerfTest()
	  //  {
			//var sw = new Stopwatch();
		 //   Load("http://localhost:5000/event/event/sobytie-1").Wait();
			//sw.Start();
		 //   var tasks = new List<Task<long>>();
		 //   for (var i = 0; i < 10; i++)
		 //   {
			//    var t = Load("http://localhost:5000/event/event/sobytie-1");
			//    //t.Wait();
			//	tasks.Add(t);
			//    tasks.Add(Load("http://localhost:5000/event/event/sobytie-2"));
		 //   }
		 //   Task.WaitAll(tasks.ToArray());
			//sw.Stop();


			//Console.WriteLine(sw.ElapsedMilliseconds);
			//foreach(var t in tasks)
			//	Console.WriteLine(t.Result);
	  //  }
	}
}
