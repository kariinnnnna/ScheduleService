using Moq;
using ScheduleServiceBusinessLogic.Helpers;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceTest.Logic
{
    public class CoreImportLogicTests
    {
        [Fact]
        public async Task ImportGroupsAsync_ShouldInsert_WhenGroupDoesNotExist()
        {
            var json = @"[
  {
    ""Id"": 101,
    ""GroupName"": ""ИВТ-301""
  }
]";

            var groupStorageMock = new Mock<IGroupStorage>();
            var teacherStorageMock = new Mock<ITeacherStorage>();

            groupStorageMock
                .Setup(x => x.GetElement(It.Is<GroupSearchModel>(m => m.CoreSystemId == 101)))
                .Returns((GroupViewModel?)null);

            var logic = new CoreImportLogic(
                groupStorageMock.Object,
                teacherStorageMock.Object,
                CreateCoreApiService(json));

            await logic.ImportGroupsAsync();

            groupStorageMock.Verify(x => x.Insert(It.Is<GroupBindingModel>(m =>
                m.Id == 0 &&
                m.CoreSystemId == 101 &&
                m.GroupName == "ИВТ-301")), Times.Once);

            groupStorageMock.Verify(x => x.Update(It.IsAny<GroupBindingModel>()), Times.Never);
        }

        [Fact]
        public async Task ImportTeachersAsync_ShouldUpdate_WhenTeacherExists_AndBuildShortName()
        {
            var json = @"[
  {
    ""Id"": 22,
    ""LastName"": ""  Сидорова  "",
    ""FirstName"": ""  Анна "",
    ""Patronymic"": "" Сергеевна ""
  }
]";

            var groupStorageMock = new Mock<IGroupStorage>();
            var teacherStorageMock = new Mock<ITeacherStorage>();

            teacherStorageMock
                .Setup(x => x.GetElement(It.Is<TeacherSearchModel>(m => m.CoreSystemId == 22)))
                .Returns(new TeacherViewModel
                {
                    Id = 5,
                    CoreSystemId = 22,
                    TeacherName = "Старое имя"
                });

            var logic = new CoreImportLogic(
                groupStorageMock.Object,
                teacherStorageMock.Object,
                CreateCoreApiService(json));

            await logic.ImportTeachersAsync();

            teacherStorageMock.Verify(x => x.Update(It.Is<TeacherBindingModel>(m =>
                m.Id == 5 &&
                m.CoreSystemId == 22 &&
                m.TeacherName == "Сидорова А С")), Times.Once);

            teacherStorageMock.Verify(x => x.Insert(It.IsAny<TeacherBindingModel>()), Times.Never);
        }

        private static CoreApiService CreateCoreApiService(string json)
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(json))
            {
                BaseAddress = new Uri("http://fake-core-api/")
            };

            return new CoreApiService(httpClient);
        }

        private sealed class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _json;

            public FakeHttpMessageHandler(string json)
            {
                _json = json;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_json, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
        }
    }
}
