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

        // Belirtilen 'id' değerine göre kategori verisini veritabanından asenkron olarak getirir.
        public async Task<GetByIdCategoryDto> GetByIdCategoryDtoAsync(string id)
        {
            // _categoryCollection koleksiyonunda, CategoryID'si 'id' ile eşleşen ilk kaydı bul ve getir.
            var values = await _categoryCollection.Find<Category>(x => x.CategoryID == id).FirstOrDefaultAsync();

            // Veritabanından gelen Category nesnesini GetByIdCategoryDto tipine dönüştür (map et) ve sonucu döndür.
            return _mapper.Map<GetByIdCategoryDto>(values);
        }

        // Güncellenmiş kategori bilgilerini alır ve veritabanındaki ilgili kaydı asenkron olarak günceller.
        public async Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            // Güncelleme için gelen DTO'yu Category tipine dönüştür.
            var values = _mapper.Map<Category>(updateCategoryDto);

            // Veritabanındaki CategoryID'si güncellenen DTO'nun CategoryID'si ile eşleşen kaydı bul ve yeni değerlerle değiştir.
            await _categoryCollection.FindOneAndReplaceAsync(x => x.CategoryID == updateCategoryDto.CategoryID, values);
        }
    }
}
