using Moq;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceTest.Logic
{
    public class DutyScheduleLogicTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenDutyPersonDoesNotExist()
        {
            var dutyScheduleStorageMock = new Mock<IDutyScheduleStorage>();
            var dutyPersonStorageMock = new Mock<IDutyPersonStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            dutyPersonStorageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m => m.Id == 10)))
                .Returns((DutyPersonViewModel?)null);

            lessonTimeStorageMock
                .Setup(x => x.GetElement(It.IsAny<LessonTimeSearchModel>()))
                .Returns(new LessonTimeViewModel
                {
                    Id = 2,
                    PairNumber = 2,
                    StartTime = new TimeSpan(10, 15, 0),
                    EndTime = new TimeSpan(11, 45, 0)
                });

            var logic = new DutyScheduleLogic(
                dutyScheduleStorageMock.Object,
                dutyPersonStorageMock.Object,
                lessonTimeStorageMock.Object);

            var model = new DutyScheduleBindingModel
            {
                Date = new DateTime(2026, 4, 23),
                LessonTimeId = 2,
                DutyPersonId = 10
            };

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(model));

            Assert.Equal("Указанный дежурный не найден", ex.Message);
            dutyScheduleStorageMock.Verify(x => x.Insert(It.IsAny<DutyScheduleBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldThrow_WhenLessonTimeDoesNotExist()
        {
            var dutyScheduleStorageMock = new Mock<IDutyScheduleStorage>();
            var dutyPersonStorageMock = new Mock<IDutyPersonStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            dutyPersonStorageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m => m.Id == 4)))
                .Returns(new DutyPersonViewModel
                {
                    Id = 4,
                    LastName = "Иванова",
                    FirstName = "Мария"
                });

            lessonTimeStorageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.Id == 99)))
                .Returns((LessonTimeViewModel?)null);

            var logic = new DutyScheduleLogic(
                dutyScheduleStorageMock.Object,
                dutyPersonStorageMock.Object,
                lessonTimeStorageMock.Object);

            var model = new DutyScheduleBindingModel
            {
                Date = new DateTime(2026, 4, 24),
                LessonTimeId = 99,
                DutyPersonId = 4
            };

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(model));

            Assert.Equal("Указанное время пары не найдено", ex.Message);
            dutyScheduleStorageMock.Verify(x => x.Insert(It.IsAny<DutyScheduleBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldInsert_WhenReferencesAreValid()
        {
            var dutyScheduleStorageMock = new Mock<IDutyScheduleStorage>();
            var dutyPersonStorageMock = new Mock<IDutyPersonStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            dutyPersonStorageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m => m.Id == 7)))
                .Returns(new DutyPersonViewModel
                {
                    Id = 7,
                    LastName = "Петрова",
                    FirstName = "Елена"
                });

            lessonTimeStorageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.Id == 3)))
                .Returns(new LessonTimeViewModel
                {
                    Id = 3,
                    PairNumber = 3,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 30, 0)
                });

            var model = new DutyScheduleBindingModel
            {
                Date = new DateTime(2026, 4, 25),
                LessonTimeId = 3,
                DutyPersonId = 7,
                Place = "Коридор 3 этажа",
                Comment = "Проверка посещаемости"
            };

            dutyScheduleStorageMock
                .Setup(x => x.Insert(model))
                .Returns(new DutyScheduleViewModel
                {
                    Id = 1,
                    Date = model.Date,
                    LessonTimeId = 3,
                    DutyPersonId = 7,
                    DutyPersonName = "Петрова Елена",
                    Place = "Коридор 3 этажа",
                    Comment = "Проверка посещаемости"
                });

            var logic = new DutyScheduleLogic(
                dutyScheduleStorageMock.Object,
                dutyPersonStorageMock.Object,
                lessonTimeStorageMock.Object);

            var result = logic.Create(model);

            Assert.NotNull(result);
            Assert.Equal(7, result!.DutyPersonId);
            Assert.Equal("Коридор 3 этажа", result.Place);
            dutyScheduleStorageMock.Verify(x => x.Insert(model), Times.Once);
        }
    }
}
