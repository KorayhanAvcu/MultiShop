using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.CategoryDtos;
using MultiShop.Catalog.Entities;
using MultiShop.Catalog.Settings;

namespace MultiShop.Catalog.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        // CategoryService sınıfında MongoDB'deki "Category" koleksiyonuna erişim sağlamak için kullanılan alanlar tanımlanıyor.
        private readonly IMongoCollection<Category> _categoryCollection; // MongoDB'deki Category koleksiyonunu temsil eder.
        private readonly IMapper _mapper; // Nesne dönüşümleri için AutoMapper arayüzüdür.

        // Constructor (yapıcı metot), CategoryService sınıfı örneği oluşturulurken çalışır.
        // Bağımlılık olarak IMapper ve IDatabaseSettings alır.
        public CategoryService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            // MongoClient sınıfı ile MongoDB bağlantısı oluşturulur.
            // _databaseSettings.ConnectionString değeri, veritabanına bağlanmak için gereken bağlantı dizesidir.
            var client = new MongoClient(_databaseSettings.ConnectionString);

            // Belirtilen veritabanı adına göre MongoDB'deki veritabanına erişilir.
            var database = client.GetDatabase(_databaseSettings.DatabaseName);

            // Veritabanındaki belirli bir koleksiyon (örneğin "Categories") alınır ve _categoryCollection alanına atanır.
            // Böylece bu koleksiyon üzerinden sorgular yapılabilir (ekle, sil, güncelle vb.).
            _categoryCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);

            // IMapper örneği sınıf içindeki _mapper alanına atanır.
            // Bu, DTO ile model arasında dönüşüm işlemleri yapmak için kullanılır.
            _mapper = mapper;
        }


        public async Task CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var value = _mapper.Map<Category>(createCategoryDto);
            await _categoryCollection.InsertOneAsync(value);
        }

        public async Task DeleteCategoryAsync(string id)
        {
            await _categoryCollection.DeleteOneAsync(x=> x.CategoryID == id);
        }

        // Tüm kategorileri veritabanından asenkron olarak getirip, ResultCategoryDto listesine dönüştüren metot.
        public async Task<List<ResultCategoryDto>> GetAllCategoriesAsync()
        {
            // MongoDB'deki _categoryCollection koleksiyonunda tüm verileri (şart olmadan) getirir.
            // x => true ifadesi, tüm belgelerin seçilmesini sağlar (filtre yok).
            var values = await _categoryCollection.Find(x => true).ToListAsync();

            // AutoMapper kullanılarak, Category türündeki veriler ResultCategoryDto türüne dönüştürülür.
            return _mapper.Map<List<ResultCategoryDto>>(values);
        }
        public Task<GetByIdCategoryDto> GetByIdCategoryDtoAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            throw new NotImplementedException();
        }
    }
}
