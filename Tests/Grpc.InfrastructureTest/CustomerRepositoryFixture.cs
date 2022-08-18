using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;

namespace Grpc.InfrastructureTest
{
    internal class CustomerRepositoryFixture
    {
        public CustomerRepositoryFixture(IEnumerable<IRepository<Customer>> customerRepositories)
        {
            var customerRepository = customerRepositories.Single(f => f.GetType() == typeof(CustomerRepositoryEf));

            var customers = GetCustomers();

            foreach (var p in customers)
                customerRepository.Insert(p);
        }

        private static IEnumerable<Customer> GetCustomers()
        {
            var result = new List<Customer>();
            var customers = _customers.Split(new[] { '\n' });

            foreach (var customer in customers)
            {
                var fields = customer.Replace(@"\n", "").Split('|');

                result.Add(new Customer
                {
                    CustomerId = Convert.ToInt32(fields[0]),
                    NameStyle = Convert.ToBoolean(Convert.ToInt32(fields[1])),
                    Title = fields[2],
                    FirstName = fields[3],
                    MiddleName = fields[4],
                    LastName = fields[5],
                    Suffix = fields[6],
                    CompanyName = fields[7],
                    SalesPerson = fields[8],
                    EmailAddress = fields[9],
                    Phone = fields[10],
                    PasswordHash = fields[11],
                    PasswordSalt = fields[12],
                    RowGuid = Guid.NewGuid(),
                    ModifiedDate = Convert.ToDateTime(fields[13])
                });
            }

            return result;
        }

        private const string _customers =
                                    @"1|0|Mr.|Orlando|N.|Gee||A Bike Store|adventure-works\pamela0|orlando0@adventure-works.com|245-555-0173|L/Rlwxzp4w7RWmEgXX+/A7cXaePEPcp+KwQhl2fJL7w=|1KjXYs4=|2005-08-01 00:00:00\n
                                    2|0|Mr.|Keith||Harris||Progressive Sports|adventure-works\david8|keith0@adventure-works.com|170-555-0127|YPdtRdvqeAhj6wyxEsFdshBDNXxkCXn+CRgbvJItknw=|fs1ZGhY=|2006-08-01 00:00:00\n
                                    3|0|Ms.|Donna|F.|Carreras||Advanced Bike Components|adventure-works\jillian0|donna0@adventure-works.com|279-555-0130|LNoK27abGQo48gGue3EBV/UrlYSToV0/s87dCRV7uJk=|YTNH5Rw=|2005-09-01 00:00:00\n
                                    4|0|Ms.|Janet|M.|Gates||Modular Cycle Systems|adventure-works\jillian0|janet1@adventure-works.com|710-555-0173|ElzTpSNbUW1Ut+L5cWlfR7MF6nBZia8WpmGaQPjLOJA=|nm7D5e4=|2006-07-01 00:00:00\n
                                    5|0|Mr.|Lucy||Harrington||Metropolitan Sports Supply|adventure-works\shu0|lucy0@adventure-works.com|828-555-0186|KJqV15wsX3PG8TS5GSddp6LFFVdd3CoRftZM/tP0+R4=|cNFKU4w=|2006-09-01 00:00:00\n
                                    6|0|Ms.|Rosmarie|J.|Carroll||Aerobic Exercise Company|adventure-works\linda3|rosmarie0@adventure-works.com|244-555-0112|OKT0scizCdIzymHHOtyJKQiC/fCILSooSZ8dQ2Y34VM=|ihWf50M=|2007-09-01 00:00:00\n
                                    7|0|Mr.|Dominic|P.|Gash||Associated Bikes|adventure-works\shu0|dominic0@adventure-works.com|192-555-0173|ZccoP/jZGQm+Xpzc7RKwDhS11YFNybwcPVRYTSNcnSg=|sPoUBSQ=|2006-07-01 00:00:00\n
                                    10|0|Ms.|Kathleen|M.|Garza||Rural Cycle Emporium|adventure-works\josé1|kathleen0@adventure-works.com|150-555-0127|Qa3aMCxNbVLGrc0b99KsbQqiVgwYDfHcsK9GZSUxcTM=|Ls05W3g=|2006-09-01 00:00:00\n
                                    11|0|Ms.|Katherine||Harding||Sharp Bikes|adventure-works\josé1|katherine0@adventure-works.com|926-555-0159|uRlorVzDGNJIX9I+ehTlRK+liT4UKRgWhApJgUMC2d4=|jpHKbqE=|2005-08-01 00:00:00\n
                                    12|0|Mr.|Johnny|A.|Caprio|Jr.|Bikes and Motorbikes|adventure-works\garrett1|johnny0@adventure-works.com|112-555-0191|jtF9jBoFYeJTaET7x+eJDkd7BzMz15Wo9odbGPBaIak=|wVLnvHo=|2006-08-01 00:00:00\n
                                    16|0|Mr.|Christopher|R.|Beck|Jr.|Bulk Discount Store|adventure-works\jae0|christopher1@adventure-works.com|1 (11) 500 555-0132|sKt9daCzEEKWAzivEGPOp8tmaM1R3I+aJfcBjzJRFLo=|8KfYx/4=|2006-09-01 00:00:00\n
                                    18|0|Mr.|David|J.|Liu||Catalog Store|adventure-works\michael9|david20@adventure-works.com|440-555-0132|61zeTkO+eI5g8GG0swny8Wp/6GzZMFnT71fnW4lTHNY=|c7Ttvv0=|2005-08-01 00:00:00\n
                                    19|0|Mr.|John|A.|Beaver||Center Cycle Shop|adventure-works\pamela0|john8@adventure-works.com|521-555-0195|DzbqWX7B3EK5Dub92CKHYSUGKGbZCbrcVDpVe/xyBeI=|zXNgrJw=|2007-04-01 00:00:00\n
                                    20|0|Ms.|Jean|P.|Handley||Central Discount Store|adventure-works\david8|jean1@adventure-works.com|582-555-0113|o1GVo3vExeNzo0/ctdRGf2eDK3uzTlcUbr18tN+Slf8=|uMsvfdo=|2005-09-01 00:00:00\n
                                    21|0||Jinghao||Liu||Chic Department Stores|adventure-works\jillian0|jinghao1@adventure-works.com|928-555-0116|IaD5AeqK9mRiIrJi/etZGVO6EiybLf/oksA2CqrpoJ0=|p6pOqKc=|2006-09-01 00:00:00\n
                                    22|0|Ms.|Linda|E.|Burnett||Travel Systems|adventure-works\jillian0|linda4@adventure-works.com|121-555-0121|23AwhujCoXYSPiN/B+G8Z9rk36xl35EbdLT7akTMTqU=|SmyIPjE=|2005-08-01 00:00:00\n
                                    23|0|Mr.|Kerim||Hanif||Bike World|adventure-works\shu0|kerim0@adventure-works.com|216-555-0122|d0WSjosAd7Y3XOWjNAkoTClCb50vwPuAawOSI1iosgs=|33g5co8=|2006-09-01 00:00:00\n
                                    24|0|Mr.|Kevin||Liu||Eastside Department Store|adventure-works\linda3|kevin5@adventure-works.com|926-555-0164|ylTpkIOHKLcjihNjS0j/k10eOHOsWQMNhlbuOCp+UTY=|TgZnUOg=|2006-09-01 00:00:00\n
                                    25|0|Mr.|Donald|L.|Blanton||Coalition Bike Company|adventure-works\shu0|donald0@adventure-works.com|357-555-0161|pKYDelLBOZMO98GBzhMxBSzzE0gUYKx9dXzYTYNuBgw=|jKtOaOw=|2006-09-01 00:00:00\n
                                    28|0|Ms.|Jackie|E.|Blackwell||Commuter Bicycle Store|adventure-works\josé1|jackie0@adventure-works.com|972-555-0163|wqhgKfOTfef4Zo3cb6FwsFzvG/yCVstuYh3AuwjRszQ=|SZ+r60o=|2007-08-01 00:00:00\n
                                    29|0|Mr.|Bryan||Hamilton||Cross-Country Riding Supplies|adventure-works\josé1|bryan2@adventure-works.com|344-555-0144|ftRWIIT4oF+if+ddn1ROYXHw6PAooKFX3uALZ0uEU98=|IRNPDIw=|2005-08-01 00:00:00\n
                                    30|0|Mr.|Todd|R.|Logan||Cycle Merchants|adventure-works\garrett1|todd0@adventure-works.com|783-555-0110|FV6z03ywMJOumcU+TEoL/Z/s4YP2fe8B3MJUUTA0CHU=|mFRhaEg=|2006-09-01 00:00:00\n
                                    34|0|Ms.|Barbara|J.|German||Cycles Wholesaler & Mfg.|adventure-works\jae0|barbara4@adventure-works.com|1 (11) 500 555-0181|2Pyd3S3Os61yt+lfSjMgSDNwT1LL4Qs51m1ob42We40=|Jvsxxrg=|2007-07-01 00:00:00\n
                                    37|0|Mr.|Jim||Geist||Two Bike Shops|adventure-works\pamela0|jim1@adventure-works.com|724-555-0161|cvqeC4fJcKwJ9jlluiWvK5/MyuSi8neLnjFDGdvzJy4=|ot8WcXk=|2006-09-01 00:00:00\n
                                    38|0|Ms.|Betty|M.|Haines||Finer Mart|adventure-works\david8|betty0@adventure-works.com|867-555-0114|Q/nGAVzOO1ZT/4+BQ1kwltqQyzycLA2GGmDmBBxLMnM=|6IvcbVg=|2007-09-01 00:00:00\n
                                    39|0|Ms.|Sharon|J.|Looney||Fitness Hotel|adventure-works\jillian0|sharon2@adventure-works.com|377-555-0132|Uo3kAuNh936QfPTIfPt6I6Z3+olLMRu5IC5awuzDaG8=|uHgb0IU=|2006-09-01 00:00:00\n
                                    40|0|Mr.|Darren||Gehring||Journey Sporting Goods|adventure-works\jillian0|darren0@adventure-works.com|417-555-0182|kqptixZ7LqTuOKcc7ylpabWrgWr5BDXa2fpkDxEwaCY=|Xe7grug=|2005-08-01 00:00:00\n
                                    41|0|Ms.|Erin|M.|Hagens||Distant Inn|adventure-works\shu0|erin1@adventure-works.com|244-555-0127|92Sfw/bl0dUJO9SAfoKn452VJcstSo2lgAbSPk97nRA=|8soIcx8=|2006-07-01 00:00:00\n
                                    42|0|Mr.|Jeremy||Los||Healthy Activity Store|adventure-works\linda3|jeremy0@adventure-works.com|911-555-0165|jLMkpmNutZFzWw7sSWZeqd91vtlRwaWx2Ub21mhsvlU=|JK9/WX8=|2006-07-01 00:00:00\n
                                    43|0|Ms.|Elsa||Leavitt||Frugal Bike Shop|adventure-works\shu0|elsa0@adventure-works.com|482-555-0174|BmJaM+147GrhU00kNTtaTolp0EJcarJK/SCfew4ZiYA=|YADhpPo=|2006-08-01 00:00:00\n";
    }
}
