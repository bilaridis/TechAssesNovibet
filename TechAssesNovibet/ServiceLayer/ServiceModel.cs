using DynamicLinkLibrary;
using DynamicLinkLibrary.Interfaces;
using DynamicLinkLibrary.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechAssesNovibet.Repository;

namespace TechAssesNovibet.ServiceLayer
{
    public enum Statuses
    {
        Registered,
        Initiating,
        Initiated,
        Ongoing,
        Completed,
        Failed
    }
    public class ServiceModel : IServiceModel
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IIPInfoProvider _provider;
        private IMemoryCache _cache;

        public ServiceModel(IMemoryCache cache, IServiceScopeFactory serviceScopeFactory, IIPInfoProvider provider)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _provider = provider;
            _cache = cache;
        }

        #region IServiceModel
        public IIPdetails GetByIP(string ipAddress)
        {
            try
            {
                _cache.TryGetValue(ipAddress, out object cacheResult);
                if (cacheResult != null)
                {
                    return (IIPdetails)cacheResult;
                }
                else
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetService<NovibetContext>();
                    var dbObject = context.IpDetails.Find(ipAddress);
                    if (dbObject != null)
                    {
                        _cache.Set(ipAddress, dbObject, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 1, 0) });
                        return dbObject;
                    }
                    else
                    {
                        IIPdetails ipStackObject = CallServiceProvider(ipAddress);
                        context.IpDetails.Add(new IpDetail(ipStackObject, ipAddress));
                        context.SaveChanges();
                        return ipStackObject;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IPServiceNotAvailableException(ex.Message);
            }
        }

        public Guid RegisterBatch(List<string> ipAddresses)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<NovibetContext>();
                var batchProcess = new BatchProcess();
                context.BatchProcesses.Add(batchProcess);
                batchProcess.Status = Statuses.Registered.ToString();
                context.SaveChanges();
                InitiateBatch(ipAddresses, batchProcess);
                return batchProcess.BatchId;
            }
            catch (Exception ex)
            {

                throw new IPServiceNotAvailableException(ex.Message);
            }
        }

        public string GetBatchProcess(Guid processId)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<NovibetContext>();
                var process = context.BatchProcesses.FirstOrDefault(x => x.BatchId == processId);
                if (process != null)
                {
                    if (!process.IsCompleted.Value)
                    {
                        var addresses = context.BatchAddresses.Where(x => x.FBatchId == processId).ToList();
                        if (addresses.Count > 0)
                        {
                            return $"Process Status : {process.Status} - { addresses.Count(x => x.IsCompleted.Value) } of { addresses.Count }";
                        }
                        else
                        {
                            return $"Process Status {process.Status} - Message : {process.ExceptionMessage}";
                        }
                    }
                    else
                    {
                        return $"Process is Completed - Status : {process.Status} - Message : {process.ExceptionMessage}";
                    }
                }
                else
                {
                    return "Process could not found";
                }
            }
            catch (Exception ex)
            {

                throw new IPServiceNotAvailableException(ex.Message);
            }

        }
        #endregion

        private IIPdetails CallServiceProvider(string ipAddress)
        {
            var ipStackObject = _provider.GetIPdetails(ipAddress);
            _cache.Set(ipAddress, ipStackObject, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 1, 0) });
            return ipStackObject;
        }

        private void InitiateBatch(List<string> ipAddresses, BatchProcess batchProcess)
        {
            var taskJob = Task.Factory.StartNew((Object obj) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<NovibetContext>();
                var objectCollection = obj as Object[];
                var batchProcessObject = objectCollection[0] as BatchProcess;
                var ListOfipAddresses = objectCollection[1] as List<string>;
                if (batchProcessObject == null) return;

                try
                {
                    UpdateBatchRow(batchProcessObject, context, Statuses.Initiating);
                    foreach (string ipAddress in ListOfipAddresses)
                    {
                        var dbObject = context.IpDetails.Find(ipAddress);
                        if (dbObject == null)
                        {
                            IIPdetails ipStackObject = CallServiceProvider(ipAddress);
                            context.BatchAddresses.Add(new BatchAddress() { IpAddress = ipAddress, FBatchId = batchProcessObject.BatchId, IsCompleted = false });
                            context.IpDetails.Add(new IpDetail(ipStackObject, ipAddress));
                        }
                        else
                        {
                            IIPdetails ipStackObject = CallServiceProvider(ipAddress);
                            context.BatchAddresses.Add(new BatchAddress() { IpAddress = ipAddress, FBatchId = batchProcessObject.BatchId, IsCompleted = false });
                            context.Entry(dbObject).CurrentValues.SetValues(new IpDetail(ipStackObject, ipAddress));
                        }
                    }

                    UpdateBatchRow(batchProcessObject, context, Statuses.Initiated, true);
                    RunBatch(batchProcessObject);
                }
                catch (Exception ex)
                {
                    UpdateBatchRow(batchProcessObject, context, Statuses.Failed, true, ex);
                    throw;
                }

            }, new object[]
              {
                  batchProcess ,
                  ipAddresses
              });
        }

        private void RunBatch(BatchProcess batchProcess)
        {
            Task.Factory.StartNew((Object obj) =>
            {
                var batchProcessObject = obj as BatchProcess;
                if (batchProcessObject == null) return;
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<NovibetContext>();
                UpdateBatchRow(batchProcessObject, context, Statuses.Ongoing);
                try
                {
                    var arrayOfAddresses = context.BatchAddresses.Where(x => x.FBatchId == batchProcess.BatchId);
                    while (arrayOfAddresses.Any())
                    {
                        var batch = arrayOfAddresses.Take(10);
                        arrayOfAddresses = arrayOfAddresses.Skip(10);

                        foreach (var item in batch)
                        {
                            var dbObject = context.IpDetails.Find(item.IpAddress);
                            if (dbObject == null)
                            {
                                IIPdetails ipStackObject = CallServiceProvider(item.IpAddress);
                                context.IpDetails.Add(new IpDetail(ipStackObject, item.IpAddress));
                            }
                            else
                            {
                                IIPdetails ipStackObject = CallServiceProvider(item.IpAddress);
                                context.Entry(dbObject).CurrentValues.SetValues(new IpDetail(ipStackObject, item.IpAddress));
                            }
                            var batchAddress = context.BatchAddresses.FirstOrDefault(x => x.IpAddress == item.IpAddress && x.FBatchId == batchProcessObject.BatchId);
                            if (batchAddress != null)
                            {
                                batchAddress.IsCompleted = true;
                            }
                        }
                        context.SaveChanges();
                    }

                    UpdateBatchRow(batchProcessObject, context, Statuses.Completed, true);
                }
                catch (Exception ex)
                {
                    UpdateBatchRow(batchProcessObject, context, Statuses.Failed, true, ex);
                    throw;
                }
            }, batchProcess);
        }

        private static void UpdateBatchRow(BatchProcess batchProcessObject, NovibetContext context, Statuses status, bool completeFlag = false, Exception ex = null, DateTime? finished = null)
        {
            batchProcessObject.IsCompleted = completeFlag;
            batchProcessObject.Status = status.ToString();
            batchProcessObject.Finished = finished;
            batchProcessObject.ExceptionMessage = ex?.Message;
            var processObject = context.BatchProcesses.Find(batchProcessObject.BatchId);
            context.Entry(processObject).CurrentValues.SetValues(batchProcessObject);
            context.SaveChanges();
        }
    }
}
