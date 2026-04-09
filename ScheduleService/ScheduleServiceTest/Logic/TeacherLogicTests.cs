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
    public class TeacherLogicTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenTeacherNameIsEmpty()
        {
            var storageMock = new Mock<ITeacherStorage>();
            var logic = new TeacherLogic(storageMock.Object);

            var ex = Assert.Throws<ArgumentException>(() => logic.Create(new TeacherBindingModel
            {
                CoreSystemId = 7,
                TeacherName = "   "
            }));

            Assert.Equal("Не указано имя преподавателя", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<TeacherBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldThrow_WhenCoreSystemIdAlreadyExists()
        {
            var storageMock = new Mock<ITeacherStorage>();

            storageMock
                .Setup(x => x.GetElement(It.Is<TeacherSearchModel>(m => m.CoreSystemId == 7)))
                .Returns(new TeacherViewModel
                {
                    Id = 2,
                    CoreSystemId = 7,
                    TeacherName = "Иванов И.И."
                });

            var logic = new TeacherLogic(storageMock.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(new TeacherBindingModel
            {
                CoreSystemId = 7,
                TeacherName = "Петров П.П."
            }));

            Assert.Equal("Преподаватель с таким CoreSystemId уже существует", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<TeacherBindingModel>()), Times.Never);
        }

        [Fact]
        public void Update_ShouldAllowSameCoreSystemId_ForSameTeacher()
        {
            var storageMock = new Mock<ITeacherStorage>();

            var model = new TeacherBindingModel
            {
                Id = 4,
                CoreSystemId = 25,
                TeacherName = "Сидорова А.А."
            };

            var existing = new TeacherViewModel
            {
                Id = 4,
                CoreSystemId = 25,
                TeacherName = "Сидорова А.А. (старое)"
            };

            storageMock
                .Setup(x => x.GetElement(It.Is<TeacherSearchModel>(m => m.Id == 4)))
                .Returns(existing);

            storageMock
                .Setup(x => x.GetElement(It.Is<TeacherSearchModel>(m => m.CoreSystemId == 25)))
                .Returns(existing);

            storageMock
                .Setup(x => x.Update(model))
                .Returns(new TeacherViewModel
                {
                    Id = 4,
                    CoreSystemId = 25,
                    TeacherName = "Сидорова А.А."
                });

            var logic = new TeacherLogic(storageMock.Object);

            var result = logic.Update(model);

            Assert.NotNull(result);
            Assert.Equal("Сидорова А.А.", result!.TeacherName);
            storageMock.Verify(x => x.Update(model), Times.Once);
        }
    }
}
