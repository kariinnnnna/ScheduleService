using Moq;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceTest.Logic
{
    public class UniversityScheduleLogicTests
    {
        [Fact]
        public void ParseGroupScheduleFromHtml_ShouldParseHeader_AndSplitTwoLessonsFromOneCell()
        {
            var scheduleStorageMock = new Mock<IScheduleItemStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            var logic = new UniversityScheduleLogic(
                scheduleStorageMock.Object,
                lessonTimeStorageMock.Object);

            var html = @"
<html>
<head><meta charset='utf-8' /></head>
<body>
    <div>Расписание занятий учебной группы: ИВТ-301</div>
    <div>Неделя: 10-я (14.04.2026 — 20.04.2026)</div>

    <table>
        <tr>
            <th>Пары</th>
            <th>1</th>
        </tr>
        <tr>
            <th>Время</th>
            <th>08:30-10:00</th>
        </tr>
        <tr>
            <th>Понедельник, 14.04.2026</th>
            <td>
                лек.<br>
                Высшая математика<br>
                Иванов И.И.<br>
                6-604<br>
                1 п/г<br>
                лаб.<br>
                Программирование<br>
                Петров П.П.<br>
                3_5<br>
                2 п/г
            </td>
        </tr>
    </table>
</body>
</html>";

            var result = logic.ParseGroupScheduleFromHtml(new UniversityScheduleParseHtmlBindingModel
            {
                HtmlContent = html
            });

            Assert.Equal("ИВТ-301", result.GroupName);
            Assert.Equal(10, result.WeekNumber);
            Assert.Equal(new DateTime(2026, 4, 14), result.WeekStartDate);
            Assert.Equal(new DateTime(2026, 4, 20), result.WeekEndDate);
            Assert.Equal(2, result.Items.Count);

            var first = result.Items[0];
            Assert.Equal("Понедельник", first.DayName);
            Assert.Equal(new DateTime(2026, 4, 14), first.Date);
            Assert.Equal(1, first.PairNumber);
            Assert.Equal("08:30-10:00", first.TimeRange);
            Assert.Equal("лек.", first.LessonType);
            Assert.Equal("Высшая математика", first.Subject);
            Assert.Equal("Иванов И.И.", first.TeacherName);
            Assert.Equal("6-604", first.ClassroomNumber);
            Assert.Equal("1 п/г", first.Subgroup);

            var second = result.Items[1];
            Assert.Equal("лаб.", second.LessonType);
            Assert.Equal("Программирование", second.Subject);
            Assert.Equal("Петров П.П.", second.TeacherName);
            Assert.Equal("3_5", second.ClassroomNumber);
            Assert.Equal("2 п/г", second.Subgroup);
        }

        [Fact]
        public void ImportGroupSchedulesFromFolder_ShouldResolveLessonTime_UseFallbacks_AndParseManualTime()
        {
            var scheduleStorageMock = new Mock<IScheduleItemStorage>();
            var lessonTimeStorageMock = new Mock<ILessonTimeStorage>();

            lessonTimeStorageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.PairNumber == 1)))
                .Returns(new LessonTimeViewModel
                {
                    Id = 7,
                    PairNumber = 1,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0)
                });

            lessonTimeStorageMock
                .Setup(x => x.GetElement(It.Is<LessonTimeSearchModel>(m => m.PairNumber == 9)))
                .Returns((LessonTimeViewModel?)null);

            var logic = new UniversityScheduleLogic(
                scheduleStorageMock.Object,
                lessonTimeStorageMock.Object);

            var html = @"
<html>
<head><meta charset='utf-8' /></head>
<body>
    <div>Расписание занятий учебной группы: ИВТ-302</div>
    <div>Неделя: 11-я (15.04.2026 — 21.04.2026)</div>

    <table>
        <tr>
            <th>Пары</th>
            <th>1</th>
            <th>9</th>
        </tr>
        <tr>
            <th>Время</th>
            <th>08:30-10:00</th>
            <th>18:00-19:30</th>
        </tr>
        <tr>
            <th>Вторник, 15.04.2026</th>
            <td>
                лек.<br>
                Физика
            </td>
            <td>
                зач.<br>
                Базы данных<br>
                2 п/г
            </td>
        </tr>
    </table>
</body>
</html>";

            var folder = Path.Combine(Path.GetTempPath(), "schedule-import-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);

            try
            {
                File.WriteAllText(Path.Combine(folder, "group1.html"), html);

                logic.ImportGroupSchedulesFromFolder(new UniversityScheduleImportFolderBindingModel
                {
                    FolderPath = folder
                });

                scheduleStorageMock.Verify(x => x.DeleteImported(), Times.Once);

                scheduleStorageMock.Verify(x => x.Insert(It.Is<ScheduleItemBindingModel>(m =>
                    m.Type == ScheduleItemType.Lecture &&
                    m.Date == DateTime.SpecifyKind(new DateTime(2026, 4, 15), DateTimeKind.Utc) &&
                    m.Subject == "Физика" &&
                    m.LessonTimeId == 7 &&
                    m.StartTime == null &&
                    m.EndTime == null &&
                    m.ClassroomNumber == "Не указана" &&
                    m.GroupName == "ИВТ-302" &&
                    m.TeacherName == "Не указан" &&
                    m.Comment == null &&
                    m.IsImported == true)), Times.Once);

                scheduleStorageMock.Verify(x => x.Insert(It.Is<ScheduleItemBindingModel>(m =>
                    m.Type == ScheduleItemType.Test &&
                    m.Date == DateTime.SpecifyKind(new DateTime(2026, 4, 15), DateTimeKind.Utc) &&
                    m.Subject == "Базы данных" &&
                    m.LessonTimeId == null &&
                    m.StartTime == new TimeSpan(18, 0, 0) &&
                    m.EndTime == new TimeSpan(19, 30, 0) &&
                    m.ClassroomNumber == "Не указана" &&
                    m.GroupName == "ИВТ-302" &&
                    m.TeacherName == "Не указан" &&
                    m.Comment == "2 п/г" &&
                    m.IsImported == true)), Times.Once);
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }
    }
}
