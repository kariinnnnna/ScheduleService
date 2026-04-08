using ScheduleServiceBusinessLogic.Helpers;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Implements
{
    public class CoreImportLogic : ICoreImportLogic
    {
        private readonly IGroupStorage _groupStorage;
        private readonly ITeacherStorage _teacherStorage;
        private readonly CoreApiService _coreApiService;

        public CoreImportLogic(
            IGroupStorage groupStorage,
            ITeacherStorage teacherStorage,
            CoreApiService coreApiService)
        {
            _groupStorage = groupStorage;
            _teacherStorage = teacherStorage;
            _coreApiService = coreApiService;
        }

        public async Task ImportGroupsAsync()
        {
            var coreGroups = await _coreApiService.GetGroupsAsync();

            foreach (var coreGroup in coreGroups)
            {
                var existing = _groupStorage.GetElement(new GroupSearchModel
                {
                    CoreSystemId = coreGroup.Id
                });

                var model = new GroupBindingModel
                {
                    CoreSystemId = coreGroup.Id,
                    GroupName = coreGroup.GroupName
                };

                if (existing == null)
                {
                    _groupStorage.Insert(model);
                }
                else
                {
                    model.Id = existing.Id;
                    _groupStorage.Update(model);
                }
            }
        }

        public async Task ImportTeachersAsync()
        {
            var coreTeachers = await _coreApiService.GetTeachersAsync();

            foreach (var coreTeacher in coreTeachers)
            {
                var existing = _teacherStorage.GetElement(new TeacherSearchModel
                {
                    CoreSystemId = coreTeacher.Id
                });

                var model = new TeacherBindingModel
                {
                    CoreSystemId = coreTeacher.Id,
                    TeacherName = BuildTeacherShortName(
                        coreTeacher.LastName,
                        coreTeacher.FirstName,
                        coreTeacher.Patronymic)
                };

                if (existing == null)
                {
                    _teacherStorage.Insert(model);
                }
                else
                {
                    model.Id = existing.Id;
                    _teacherStorage.Update(model);
                }
            }
        }

        public async Task ImportAllAsync()
        {
            await ImportGroupsAsync();
            await ImportTeachersAsync();
        }

        private static string BuildTeacherShortName(string lastName, string firstName, string? patronymic)
        {
            var firstInitial = string.IsNullOrWhiteSpace(firstName)
                ? string.Empty
                : firstName.Trim()[0].ToString();

            var patronymicInitial = string.IsNullOrWhiteSpace(patronymic)
                ? string.Empty
                : patronymic.Trim()[0].ToString();

            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                parts.Add(lastName.Trim());
            }

            if (!string.IsNullOrWhiteSpace(firstInitial))
            {
                parts.Add(firstInitial);
            }

            if (!string.IsNullOrWhiteSpace(patronymicInitial))
            {
                parts.Add(patronymicInitial);
            }

            return string.Join(" ", parts);
        }
    }
}
