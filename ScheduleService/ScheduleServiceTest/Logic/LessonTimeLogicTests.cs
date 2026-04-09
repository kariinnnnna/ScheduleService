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
    public class LessonTimeLogicTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenPairNumberIsInvalid()
        {
            var storageMock = new Mock<ILessonTimeStorage>();
            var logic = new LessonTimeLogic(storageMock.Object);

            var ex = Assert.Throws<ArgumentException>(() => logic.Create(new LessonTimeBindingModel
            {
                PairNumber = 0,
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(10, 0, 0),
                Description = "Первая пара"
            }));

            Assert.Equal("Некорректный номер пары", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<LessonTimeBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldThrow_WhenStartTimeIsNotEarlierThanEndTime()
        {
            var storageMock = new Mock<ILessonTimeStorage>();
            var logic = new LessonTimeLogic(storageMock.Object);

            var ex = Assert.Throws<ArgumentException>(() => logic.Create(new LessonTimeBindingModel
            {
                PairNumber = 1,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                Description = "Некорректная пара"
            }));

            Assert.Equal("Время начала должно быть меньше времени окончания", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<LessonTimeBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldThrow_WhenPairNumberAlreadyExists()
        {
            var storageMock = new Mock<ILessonTimeStorage>();

            storageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.PairNumber == 2)))
                .Returns(new LessonTimeViewModel
                {
                    Id = 8,
                    PairNumber = 2,
                    StartTime = new TimeSpan(10, 15, 0),
                    EndTime = new TimeSpan(11, 45, 0)
                });

            var logic = new LessonTimeLogic(storageMock.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(new LessonTimeBindingModel
            {
                PairNumber = 2,
                StartTime = new TimeSpan(12, 0, 0),
                EndTime = new TimeSpan(13, 30, 0),
                Description = "Дубликат номера"
            }));

            Assert.Equal("Пара с таким номером уже существует", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<LessonTimeBindingModel>()), Times.Never);
        }

        [Fact]
        public void Update_ShouldAllowSamePairNumber_ForSameEntity()
        {
            var storageMock = new Mock<ILessonTimeStorage>();

            var model = new LessonTimeBindingModel
            {
                Id = 3,
                PairNumber = 1,
                StartTime = new TimeSpan(8, 40, 0),
                EndTime = new TimeSpan(10, 10, 0),
                Description = "Обновлённая первая пара"
            };

            var existing = new LessonTimeViewModel
            {
                Id = 3,
                PairNumber = 1,
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(10, 0, 0),
                Description = "Старая первая пара"
            };

            storageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.Id == 3)))
                .Returns(existing);

            storageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.PairNumber == 1)))
                .Returns(existing);

            storageMock
                .Setup(x => x.Update(model))
                .Returns(new LessonTimeViewModel
                {
                    Id = 3,
                    PairNumber = 1,
                    StartTime = new TimeSpan(8, 40, 0),
                    EndTime = new TimeSpan(10, 10, 0),
                    Description = "Обновлённая первая пара"
                });

            var logic = new LessonTimeLogic(storageMock.Object);

            var result = logic.Update(model);

            Assert.NotNull(result);
            Assert.Equal("Обновлённая первая пара", result!.Description);
            storageMock.Verify(x => x.Update(model), Times.Once);
        }
    }
}
