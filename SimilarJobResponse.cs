namespace Unity.Areas.Jobs.Models.Responses
{
    public class SimilarJobResponse
    {
        public int PMID { get; set; }
        public string Location { get; set; }
        public string Community { get; set; }
        public int? OfficeID { get; set; }
        public string State { get; set; }
        public string CustJobNumber { get; set; }
        public string ProjectManagerUserId { get; set; }
        public DateTime? OpenDate { get; set; }
        public string OfficeName => OfficeID.HasValue ? Office.getOfficeDTOFromId((int)OfficeID)?.OfficeName : null;
        public string ProjectManagerName => ProjectManagerUserId.Any() ? User.getUserDTOFromUserId(ProjectManagerUserId)?.fullName : null;
        public string OpenDateString => OpenDate?.ToShortDateString() ?? string.Empty;
        public int? NumberOfServiceOrders { get; set; }
        public string LastServiceDate { get; set; }
        public string CustomerName { get; set; }

        public void InitSimilarJobResponse()
        {
            InitNumberOfServiceOrders();
            InitLastServiceDate();
            InitCustomer();
        }

        public void InitNumberOfServiceOrders()
        {
            using (var db = new TMIEntities())
            {
                NumberOfServiceOrders = db.ServiceOrders.Where(so => so.PMID == PMID).ToList().Count();
            }
        }
        public void InitLastServiceDate()
        {
            using (var db = new TMIEntities())
            {
                var lastServiceDate = db.ServiceOrders
                                        .Where(so => so.PMID == PMID)
                                        .Max(so => so.OrderDate);

                LastServiceDate = lastServiceDate?.ToShortDateString() ?? string.Empty;
            }
        }
        public void InitCustomer()
        {
            using (var db = new TMIEntities())
            {
                var cust = db.PMCustomers.Include(c => c.Customer_Info).FirstOrDefault(c => c.PMID == this.PMID);
                if (cust != null)
                {
                    var customer = new CustomerDTO(cust.Customer_Info);
                    CustomerName = customer.Company_Name;
                }
            }
        }
    }
