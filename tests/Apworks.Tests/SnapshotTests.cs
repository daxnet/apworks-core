using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Apworks.Tests
{
    public class SnapshotTests
    {
        [Fact]
        public void SaveSnapshotTest()
        {
            var id = Guid.NewGuid();
            var employee = new Employee(id);
            employee.ChangeName("daxnet");
            employee.ChangeTitle("Developer");
            var snapshot = employee.TakeSnapshot();
            Assert.Equal(4, snapshot.States.Count());
            Assert.Equal(3, snapshot.Version);
        }

        [Fact]
        public void SnapshotValueTest()
        {
            var id = Guid.NewGuid();
            var employee = new Employee(id);
            employee.ChangeName("daxnet");
            employee.ChangeTitle("Developer");
            var snapshot = employee.TakeSnapshot();
            var states = (Dictionary<string, object>)snapshot.States;
            Assert.Equal("daxnet", states["Name"]);
            Assert.Equal("Sr. Developer", states["Title"]);
        }

        [Fact]
        public void RestoreSnapshotTest()
        {
            var id = Guid.NewGuid();
            var employee = new Employee(id);
            employee.ChangeName("daxnet");
            employee.ChangeTitle("Developer");
            var snapshot = employee.TakeSnapshot();

            var employee2 = new Employee(Guid.Empty);
            employee2.RestoreSnapshot(snapshot);

            Assert.Equal(employee.Id, employee2.Id);
            Assert.Equal(employee.Version, employee2.Version);
            Assert.Equal(employee.Title, employee2.Title);
            Assert.Equal(employee.DateRegistered, employee2.DateRegistered);
        }
    }
}
