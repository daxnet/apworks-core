using Apworks.Repositories.MongoDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Tests.Integration.Fixtures
{
    public class MongoDBFixture
    {
        public static readonly object locker = new object();


        public MongoDBFixture()
        {
            this.Settings = new MongoRepositorySettings("localhost", "apworks-integration-test");
        }

        public MongoRepositorySettings Settings { get; }
    }
}
