using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using WebAPI.Contexts;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Models.Concrete;
using WebAPI.Models.Constants;
using WebAPI.Models.Dtos.UserDtos;
using WebAPI.Models.Dtos.WarehouseLocationProblemDtos;
using WebAPI.Results.Abstract;
using WebAPI.Results.Concrete;
using static WebAPI.Models.Constants.SimulatedAnnealingConstants;
using static WebAPI.Models.Constants.ResponseDescriptions;
using WebAPI.CrossCuttingConcerns.Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationSolverController : ControllerBase
    {
        private readonly IMapper _mapper;
        private Random _random = new Random(); // Random değer atamak için
        private int _numWarehouses { get; set; }
        private int _numCustomers { get; set; }
        private List<Customer> _customers;  // Müşteri listesi
        private List<Warehouse> _warehouses;  // Depo listesi
        private Dictionary<int, int> _solution; // _solution veri tipi Dictionary<int, int> olarak değiştirildi
        public Dictionary<int, int> Solution // Solution property'sinin veri tipi değiştirildi
        {
            get { return _solution; }
            private set { _solution = value; }
        }

        public LocationSolverController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [SwaggerOperation(description: ResponseDescriptions.EXCEPTION_DETAIL)]
        [HttpPost("locationsolver")]
        public async Task<IActionResult> LocationSolver()
        {
            try
            {
                using (BaseDbContext context = new BaseDbContext())
                {
                    _customers = await context.Customers
                                                  .AsNoTracking()
                                                  .Include(c => c.CustomerWarehouseCosts)
                                                  .ToListAsync();
                    _numCustomers = _customers.Count;
                    
                    _warehouses = await context.Warehouses
                                                  .AsNoTracking()
                                                  .ToListAsync();
                    _numWarehouses = _warehouses.Count;
                    if (_numCustomers <= 0) throw new BusinessException(CUSTOMER_NOT_FOUND);
                    if ( _numWarehouses <= 0) throw new BusinessException(WAREHOUSE_NOT_FOUND);
                }

                Dictionary<int, int> currentSolution = generateRandomSolution(); // currentSolution veri tipi değiştirildi
                double currentCost = calculateCost(currentSolution.Values.ToList());

                Dictionary<int, int> bestSolution = new Dictionary<int, int>(currentSolution); // bestSolution veri tipi değiştirildi
                double bestCost = currentCost;

                double temperature = INITAL_TEMPERATURE;
                int iteration = 0;

                while (temperature > 0 && iteration < MAX_ITERATIONS)
                {
                    Dictionary<int, int> newSolution = generateNeighborSolution(currentSolution); // newSolution veri tipi değiştirildi
                    double newCost = calculateCost(newSolution.Values.ToList());

                    if (shouldAcceptNewSolution(currentCost, newCost))
                    {
                        currentSolution = new Dictionary<int, int>(newSolution);
                        currentCost = newCost;
                    }
                    if (isNewCostBetter(newCost, bestCost))
                    {
                        bestSolution = new Dictionary<int, int>(newSolution);
                        bestCost = newCost;
                    }
                    temperature *= COOLINGRATE;
                    iteration++;
                }

                return Ok(new SuccessDataResult<BestResult>(new BestResult(bestCost, bestSolution.Values.ToList())));
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorModel()
                {
                    Type = "https://example.com/probs/business",
                    Title = "Rule Validation",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = ""
                });

            }
            catch (Exception ex)
            {
                // Diğer hata durumları...
                return StatusCode(500, new { message = SERVER_ERROR });
            }
        }

        private double calculateCost(List<int> solution)
        {
            double totalCost = 0;
            for (int i = 0; i < _numCustomers; i++)
            {
                totalCost += CalculateCostForSelectedWarehouse(solution[i], i);
            }

            return totalCost;
        }
        private Dictionary<int, int> generateRandomSolution()
        {
            List<Customer> customers = new List<Customer>(_customers);
            List<Warehouse> warehouses = new List<Warehouse>(_warehouses);
            Dictionary<int, int> solution = new Dictionary<int, int>();

            sortBy(customers, 0, customers.Count - 1, compareCustomersByDemand);
            sortBy(warehouses, 0, warehouses.Count - 1, compareWarehousesBySetupCost);

            foreach (var customer in customers)
            {
                Warehouse selectedWarehouse = findSuitableWarehouse(customer, warehouses);
                if (selectedWarehouse is not null)
                {
                    selectedWarehouse.Capacity -= customer.Demand;
                    solution[customer.Id] = selectedWarehouse.Id;
                }
                else
                {
                    // Uygun bir depo bulunamadı
                }
            }

            return solution;
        }

        private Dictionary<int, int> generateNeighborSolution(Dictionary<int, int> currentSolution)
        {
            Dictionary<int, int> newSolution = new Dictionary<int, int>(currentSolution);
            int randomCustomerId = _customers[_random.Next(_numCustomers)].Id;

            // Rastgele bir depo ID'si seç
            int randomWarehouseId = _warehouses[_random.Next(_numWarehouses)].Id;

            newSolution[randomCustomerId] = randomWarehouseId;

            return newSolution;
        }
        private double CalculateCostForSelectedWarehouse(int warehouseId, int customerId)
        {
            CustomerWarehouseCost? travel = _customers[customerId].CustomerWarehouseCosts.Where(p => p.WarehouseID == warehouseId).FirstOrDefault();
            Warehouse? setup = _warehouses.Where(p => p.Id == warehouseId).FirstOrDefault();

            if (travel is not null && setup is not null)
            {
                double travelCost = travel.Cost;
                double setupCost = setup.SetupCost;
                return travelCost + setupCost;
            }
            else return 0;
        }
        private bool shouldAcceptNewSolution(double currentCost, double newCost)
        {
            if (isNewCostBetterThanCurrent(newCost, currentCost)) return true;

            double acceptanceProbability = calculateAcceptanceProbability(currentCost, newCost);
            double randomValue = _random.NextDouble();

            return randomValue < acceptanceProbability;
        }
        private Warehouse findSuitableWarehouse(Customer customer, List<Warehouse> warehouses) => warehouses.FirstOrDefault(item => isCapacitySufficient(item.Capacity, customer.Demand));
        private bool isNewCostBetter(double newCost, double bestCost) => newCost < bestCost;
        private bool isNewCostBetterThanCurrent(double newCost, double currentCost) => newCost < currentCost;
        private bool isCapacitySufficient(int capacity, int demand) => capacity >= demand;
        private double calculateAcceptanceProbability(double currentCost, double newCost) => Math.Exp((currentCost - newCost) / INITAL_TEMPERATURE);
        private void sortBy<T>(List<T> list, int low, int high, Comparison<T> comparison)
        {
            if (low < high)
            {
                int partitionIndex = partition(list, low, high, comparison);
                sortBy(list, low, partitionIndex - 1, comparison);
                sortBy(list, partitionIndex + 1, high, comparison);
            }
        }
        private int compareCustomersByDemand(Customer c1, Customer c2)
        {
            return c1.Demand.CompareTo(c2.Demand);
        }

        private int compareWarehousesBySetupCost(Warehouse w1, Warehouse w2)
        {
            return w1.SetupCost.CompareTo(w2.SetupCost);
        }
        private int partition<T>(List<T> list, int low, int high, Comparison<T> comparison)
        {
            T pivot = list[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (comparison.Invoke(list[j], pivot) <= 0)
                {
                    i++;
                    Swap(list, i, j);
                }
            }

            Swap(list, i + 1, high);
            return i + 1;
        }

        private void Swap<T>(List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        private bool checkCapacityConstraints(List<int> solution, List<Warehouse> warehouses, List<Customer> customers)
        {
            int warehousesCount = warehouses.Count;
            int[] warehouseCapacities = new int[warehousesCount];
            foreach (var customer in customers)
            {
                int warehouseIndex = solution[customer.Id];
                warehouseCapacities[warehouseIndex] += customer.Demand;
            }
            for (int i = 0; i < warehouses.Count; i++)
            {
                if (warehouseCapacities[i] > warehouses[i].Capacity)
                {
                    return false;
                }
            }
            for (int i = 0; i < warehousesCount; i++)
            {
                if (warehouseCapacities[i] > warehouses[i].Capacity)
                    Console.WriteLine($"Kapasite={warehouses[i].Capacity} Dolan Kapasite= {warehouseCapacities[i]}");
            }
            return true;
        }
    }
}
