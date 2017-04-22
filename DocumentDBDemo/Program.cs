using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DocumentDBDemo
{
    public class Program
    {
        //Azure connection specific properties    
        public const string databaseURL = "https://demo96.documents.azure.com:443/";
        public const string primaryKey = "Gen2EaJWSEVF3xO3OwLamJV4WsgGzmMUsD9G76P4XMrtmnvgEIrvLpdIPuarnPWaREhl2xCyQ1AdCPTdLMCVhw==";
        //Local Client
        public DocumentClient client;
        
        static void Main(string[] args)
        {
            Program P = new Program();
            try
            {
                P.InitializeDataBase().Wait();
                P.CreateDocument().Wait();
                P.ReadDocument("Products", "Cars", "1").Wait();
                P.UpdateDocument("Products", "Cars", "1").Wait();
                P.DeleteDocument("Products", "Cars", "4").Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Issue connecting to azure instance");
                throw ex;
            }
        }

        private async Task InitializeDataBase()
        {                       
            this.client = new DocumentClient(new Uri(databaseURL), primaryKey);
            await this.CreateDataBaseIfNotExists("Products");
            await this.CreateCollectionIfNotExists("Products", "Cars");
        }            

        #region CreateDatabase
        /// <summary>
        /// CreateDataBaseIfNotExists
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        private async Task CreateDataBaseIfNotExists(string databaseName)
        {
            try
            {
                await this.client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
                Console.WriteLine($"{databaseName} exists, Press any key to continue");
                Console.ReadKey();
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDatabaseAsync(new Database { Id = databaseName });
                    Console.WriteLine($"Created database {databaseName} Press any key to continue");
                    Console.ReadKey();
                }
            }
        }
        #endregion

        #region CreateCollection
        /// <summary>
        /// CreateCollectionIfNotExists
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        private async Task CreateCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await this.client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
                Console.WriteLine($"{collectionName} exists");
                Console.ReadKey();
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;
                    await this.client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName });
                    Console.WriteLine($"Created collection {collectionName} press any key to continue");
                    Console.ReadKey();
                }

            }
        }
        #endregion

        #region CreateOperation
        /// <summary>
        /// CreateDocument
        /// </summary>
        /// <returns></returns>
        private async Task CreateDocument()
        {
            //Create sample document 1
            var verna = new Car
            {
                ID = "1",
                Name = "Verna",
                Manufacturer = " Hyundai",
                ManufactureYear = new DateTime(2017, 01, 01),
                BasePrice = new decimal(800000)
            };
            await CreateDocumentIfNotExists("Products", "Cars", verna);

            //Create sample document 2
            var ciaz = new Car
            {
                ID = "2",
                Name = "Ciaz",
                Manufacturer = "Maruti",
                ManufactureYear = new DateTime(2016, 01, 01),
                BasePrice = new decimal(1000000)
            };
            await CreateDocumentIfNotExists("Products", "Cars", ciaz);

            var baleno = new Car
            {
                ID = "3",
                Name = "Baleno",
                Manufacturer = "Maruti",
                ManufactureYear = new DateTime(2016, 01, 01),
                BasePrice = new decimal(500000)
            };
            await CreateDocumentIfNotExists("Products", "Cars", baleno);

            var xcent = new Car
            {
                ID = "4",
                Name = "xcent",
                Manufacturer = "Hyundai",
                ManufactureYear = new DateTime(2016, 01, 01),
                BasePrice = new decimal(600000)
            };
            await CreateDocumentIfNotExists("Products", "Cars", xcent);


        }

        /// <summary>
        /// Implements documentdb method to create documents
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="car"></param>
        /// <returns></returns>
        private async Task CreateDocumentIfNotExists(string databaseName, string collectionName, Car car)
        {
            Console.WriteLine("Create Operation");
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, car.ID));
                Console.WriteLine($"Document {car.ID} already exists, press any key to continue");
                Console.ReadKey();
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), car);
                    Console.WriteLine($"created {car.ID}, press any key to continue");
                    Console.ReadKey();
                }
                else
                {
                    throw;
                }
            }

        }
        #endregion

        #region ReadOperation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        private async Task<Car> ReadDocument(string databaseName, string collectionName, string documentId)
        {
            Console.WriteLine("Read Operation");
            Car result;
            try
            {
                result = await this.client.ReadDocumentAsync<Car>(UriFactory.CreateDocumentUri(databaseName, collectionName, documentId));
                Console.WriteLine($"Read document {documentId} whose manufacturer is {result.Manufacturer} , press any key to continue");
                Console.ReadKey();
            }
            catch (DocumentClientException ex)
            {
                throw;
            }
            return result;
        }
        #endregion

        #region UpdateOperation
        /// <summary>
        /// UpdateDocument - Update any property of the document
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        private async Task UpdateDocument(string databaseName, string collectionName, string documentId)
        {
            Console.WriteLine("Update Operation");
            try
            {
                Car result = await ReadDocument(databaseName, collectionName, documentId);
                result.Manufacturer = $"Updated {result.Manufacturer}";

                await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, documentId), result);
                Car updatedRresult = await ReadDocument(databaseName, collectionName, documentId);
                Console.ReadKey();
            }
            catch (DocumentClientException ex)
            {
                throw;
            }
        }
        #endregion

        #region DeleteOperation
        /// <summary>
        /// DeleteDocument - Delete by passing document ID
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        private async Task DeleteDocument(string databaseName, string collectionName, string documentId)
        {
            Console.WriteLine("Delete Operation");
            try
            {
                await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, documentId));
                Console.WriteLine($"Deleted document {documentId} , press any key to continue");
                Console.ReadKey();
            }
            catch (DocumentClientException ex)
            {
                throw;
            }
        }
        #endregion

    }
}
