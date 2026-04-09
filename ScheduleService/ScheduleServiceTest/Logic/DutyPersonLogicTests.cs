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
    public class DutyPersonLogicTests
    {
        [Fact]
        public void Create_ShouldTrimFields_AndInsertNormalizedModel()
        {
            var storageMock = new Mock<IDutyPersonStorage>();

            storageMock
                .Setup(x => x.GetElement(It.IsAny<DutyPersonSearchModel>()))
                .Returns((DutyPersonViewModel?)null);

            storageMock
                .Setup(x => x.Insert(It.Is<DutyPersonBindingModel>(m =>
                    m.LastName == "Иванова" &&
                    m.FirstName == "Анна" &&
                    m.Position == "Лаборант" &&
                    m.Phone == "+79990001122" &&
                    m.Email == "anna@test.ru")))
                .Returns(new DutyPersonViewModel
                {
                    Id = 1,
                    LastName = "Иванова",
                    FirstName = "Анна",
                    Position = "Лаборант",
                    Phone = "+79990001122",
                    Email = "anna@test.ru"
                });

            var logic = new DutyPersonLogic(storageMock.Object);

            var model = new DutyPersonBindingModel
            {
                LastName = "  Иванова  ",
                FirstName = "  Анна  ",
                Position = "  Лаборант  ",
                Phone = "  +79990001122  ",
                Email = "  anna@test.ru  "
            };

            var result = logic.Create(model);

            Assert.NotNull(result);
            Assert.Equal("Иванова", model.LastName);
            Assert.Equal("Анна", model.FirstName);
            Assert.Equal("Лаборант", model.Position);
            Assert.Equal("+79990001122", model.Phone);
            Assert.Equal("anna@test.ru", model.Email);

            storageMock.Verify(x => x.Insert(It.IsAny<DutyPersonBindingModel>()), Times.Once);
        }

        [Fact]
        public void Create_ShouldThrow_WhenPhoneBelongsToAnotherDutyPerson()
        {
            var storageMock = new Mock<IDutyPersonStorage>();

            storageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m =>
                    m.LastName == "Петрова" && m.FirstName == "Мария")))
                .Returns((DutyPersonViewModel?)null);

            storageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m =>
                    m.Phone == "+79990001122")))
                .Returns(new DutyPersonViewModel
                {
                    Id = 5,
                    LastName = "Смирнова",
                    FirstName = "Ольга",
                    Phone = "+79990001122"
                });

            var logic = new DutyPersonLogic(storageMock.Object);

            var model = new DutyPersonBindingModel
            {
                LastName = "Петрова",
                FirstName = "Мария",
                Phone = "+79990001122",
                Email = "maria@test.ru"
            };

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(model));

            Assert.Equal("Дежурный с таким телефоном уже существует", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<DutyPersonBindingModel>()), Times.Never);
        }

        [Fact]
        public void Update_ShouldAllowSameEmail_ForSameEntity()
        {
            var storageMock = new Mock<IDutyPersonStorage>();

            var model = new DutyPersonBindingModel
            {
                Id = 3,
                LastName = "Сидорова",
                FirstName = "Елена",
                Position = "Староста",
                Phone = "+79995554433",
                Email = "same@test.ru"
            };

            var existing = new DutyPersonViewModel
            {
                Id = 3,
                LastName = "Сидорова",
                FirstName = "Елена",
                Position = "Помощник",
                Phone = "+79995554433",
                Email = "same@test.ru"
            };

            storageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m => m.Id == 3)))
                .Returns(existing);

            storageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m =>
                    m.LastName == "Сидорова" && m.FirstName == "Елена")))
                .Returns(existing);

            storageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m =>
                    m.Phone == "+79995554433")))
                .Returns(existing);

            storageMock
                .Setup(x => x.GetElement(It.Is<DutyPersonSearchModel>(m =>
                    m.Email == "same@test.ru")))
                .Returns(existing);

            storageMock
                .Setup(x => x.Update(model))
                .Returns(new DutyPersonViewModel
                {
                    Id = 3,
                    LastName = "Сидорова",
                    FirstName = "Елена",
                    Position = "Староста",
                    Phone = "+79995554433",
                    Email = "same@test.ru"
                });

            var logic = new DutyPersonLogic(storageMock.Object);

            var result = logic.Update(model);

            Assert.NotNull(result);
            Assert.Equal("Староста", result!.Position);
            storageMock.Verify(x => x.Update(model), Times.Once);
        }
    }
}
