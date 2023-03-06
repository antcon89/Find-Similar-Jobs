        [HttpPost]
        [Route("similar")]
        public async Task<JsonResult> GetSimilarJobs(SimilarJobRequest similarJobRequest)
        {
            var result = new TMListResult<SimilarJobResponse>();

            using (var db = new TMIEntities())
            {
                var similarJobs = await db.Jobs.Where(x => x.StatusId == (int)Statuses.Active &&
                                    x.CurrentCustomerID == similarJobRequest.Contractor.Company_ID &&
                                    (x.Location == similarJobRequest.Address) &&
                                    (similarJobRequest.State == null || x.state == similarJobRequest.State) &&
                                    (similarJobRequest.CustomerJobReferenceNumber == null || x.custJobNumber == similarJobRequest.CustomerJobReferenceNumber))
                                    .OrderByDescending(x => x.PMOpenDate)
                                    .Select(x => new SimilarJobResponse
                                    {
                                        PMID = x.JobId,
                                        OfficeID = x.OfficeID,
                                        Location = x.Location,
                                        Community = x.Community,
                                        State = x.state,
                                        CustJobNumber = x.custJobNumber,
                                        ProjectManagerUserId = x.PMUser,
                                        OpenDate = x.PMOpenDate
                                    }).Take(4).ToListAsync();

                similarJobs.ForEach(simJob => simJob.InitSimilarJobResponse());

                result.items = similarJobs;
                result.success = true;
            }

            return Json(result);
        }
