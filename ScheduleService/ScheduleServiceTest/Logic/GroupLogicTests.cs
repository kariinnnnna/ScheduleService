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
    public class GroupLogicTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenGroupNameIsEmpty()
        {
            var storageMock = new Mock<IGroupStorage>();
            var logic = new GroupLogic(storageMock.Object);

            var ex = Assert.Throws<ArgumentException>(() => logic.Create(new GroupBindingModel
            {
                CoreSystemId = 10,
                GroupName = "   "
            }));

            Assert.Equal("Не указано название группы", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<GroupBindingModel>()), Times.Never);
        }

        [Fact]
        public void Create_ShouldThrow_WhenCoreSystemIdAlreadyExists()
        {
            var storageMock = new Mock<IGroupStorage>();

            storageMock
                .Setup(x => x.GetElement(It.Is<GroupSearchModel>(m => m.CoreSystemId == 10)))
                .Returns(new GroupViewModel
                {
                    Id = 1,
                    CoreSystemId = 10,
                    GroupName = "ИВТ-101"
                });

            var logic = new GroupLogic(storageMock.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => logic.Create(new GroupBindingModel
            {
                CoreSystemId = 10,
                GroupName = "ИВТ-102"
            }));

            Assert.Equal("Группа с таким CoreSystemId уже существует", ex.Message);
            storageMock.Verify(x => x.Insert(It.IsAny<GroupBindingModel>()), Times.Never);
        }

        [Fact]
        public void Update_ShouldAllowSameCoreSystemId_ForSameEntity()
        {
            var storageMock = new Mock<IGroupStorage>();

            var model = new GroupBindingModel
            {
                Id = 5,
                CoreSystemId = 100,
                GroupName = "ПМИ-301"
            };

            var existing = new GroupViewModel
            {
                Id = 5,
                CoreSystemId = 100,
                GroupName = "ПМИ-300"
            };

            storageMock
                .Setup(x => x.GetElement(It.Is<GroupSearchModel>(m => m.Id == 5)))
                .Returns(existing);

            storageMock
                .Setup(x => x.GetElement(It.Is<GroupSearchModel>(m => m.CoreSystemId == 100)))
                .Returns(existing);

            storageMock
                .Setup(x => x.Update(model))
                .Returns(new GroupViewModel
                {
                    Id = 5,
                    CoreSystemId = 100,
                    GroupName = "ПМИ-301"
                });

            var logic = new GroupLogic(storageMock.Object);

            var result = logic.Update(model);

            Assert.NotNull(result);
            Assert.Equal("ПМИ-301", result!.GroupName);
            storageMock.Verify(x => x.Update(model), Times.Once);
        }
    }
}
