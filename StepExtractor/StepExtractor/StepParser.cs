using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StepExtractor
{
    /// <summary>
    /// Класс для обработки STEP файла
    /// </summary>
    internal class StepParser
    {
        /// <summary>
        /// Регулярное выражение для нахождения трех групп в строке: номер сущности, имя сущности, параметры сущности
        /// </summary>
        private const String PATTERN_PRIMARY = @"\s*\#\s*(\d+)\s*=\s*(.+?)\s*\(\s*(.*)\s*\)";

        private const String PATTERN_PRIMARY_PARAMS_WITH_BRACKETS = @"\s*\(\s*(.+?)\s*\)\s*";

        private const String REPLACEMENT_NAME_PART = "STEP_PARSER_REPLACED_";

        private const String PATTERN_SECONDARY_PARAMS_WITH_COMMAS = @"\s*(.+?)\s*,\s*"; //Необходима дополнительная запятая в конце строки

        /// <summary>
        /// Номер группы, в которой содержится номер сущности
        /// </summary>
        private const int MATCH_NUMBER = 1;
        /// <summary>
        /// Номер группы, в которой содержится имя сущности
        /// </summary>
        private const int MATCH_NAME = 2;
        /// <summary>
        /// Номер группы, в которой содержатся все параметры сущности в виде одной строки (требует дальнейшей обработки)
        /// </summary>
        private const int MATCH_PARAMS = 3;

        /// <summary>
        /// Чтение STEP файла и извлечение из каждой текстовой строки трех составляющих сущности: номер, название, параметры
        /// </summary>
        /// <param name="path">Путь к файлу на диске</param>
        /// <returns>Возвращает список базовых сущностей</returns>
        public List<BaseEntity> getAllPrimaryEntities(String path, IExtractOperationsCallback callback)
        {
            //Объявление пустого списка базовых сущностей
            List<BaseEntity> allPrimaryEntities = new List<BaseEntity>();

            //Проверки на правильность пути и существование файла
            if (path != null && path.Length > 0 && File.Exists(path))
            {
                //Строка буфер                
                String line;

                // Чтение файла и обработка его по строкам
                using (StreamReader file = new StreamReader(path, Encoding.Default))
                {
                    //"Компиляция" объекта регулярного выражения на основании текстового представления
                    Regex regex = new Regex(PATTERN_PRIMARY);

                    int currentStep = 5;
                    int progressStep = 0;
                    if (callback != null)
                    {
                        int countLinesInFile = System.IO.File.ReadAllLines(path).Length;
                        progressStep = countLinesInFile > 60 ? countLinesInFile / 60 : 60 / countLinesInFile;
                    }

                    //Цикл выполняется до тех пор, пока результат чтения строк из файла не станет null
                    while ((line = file.ReadLine()) != null)
                    {
                        //Проверка строки на ненулевую длину
                        if (line.Length > 0)
                        {
                            //Получение результата применения регулярного выражения regex к строке line
                            Match match = regex.Match(line);
                            if (!match.Success)
                            {
                                //Если совпадений не найдено, переход к следующей строке
                                continue;
                            }

                            //Получение всех групп результата регулярного выражения
                            //Всего 4 группы, первая группа (0) содержит всю строку, вторая группа (1) содержит номер, 
                            //третья группа (2) содержит название, четвертая группа (3) содержит строку параметров
                            //Необходимые номера групп (1,2,3) объявлены выше как константы
                            GroupCollection matchGroups = match.Groups;

                            //Создание новой базовой сущности (для дальнейшего заполнения)
                            BaseEntity baseEntity = new BaseEntity();

                            //Попытка получения номера группы в виде числа из строкового представления
                            int number = -1;
                            if (int.TryParse(matchGroups[MATCH_NUMBER].Value, out number))
                            {
                                //Сохранение полученного номера в сущности
                                baseEntity.Number = number;
                            }
                            else
                            {
                                //В случае ошибки переход к следующей строке файла
                                continue;
                            }

                            //Получение и сохраенение имени сущности
                            baseEntity.Name = matchGroups[MATCH_NAME].Value;

                            //Получение и сохранение параметров сущности
                            baseEntity.Params = new EntityParam(matchGroups[MATCH_PARAMS].Value);

                            //Разбор сторки парметров на составляющие и сохранение их в виде List<List<Параметр>>
                            baseEntity.ParsedParams = parseParams(baseEntity.Params);

                            //Добавление сущности в список сущностей
                            allPrimaryEntities.Add(baseEntity);
                        }

                        if (callback != null)
                        {
                            currentStep += progressStep;
                            callback.extractionStep(currentStep < 65 ? currentStep : 65);
                        }
                    }
                }
            }
            //Возвращение заполненного списка базовых сущностей
            return allPrimaryEntities;
        }

        /// <summary>
        /// Парсинг параметров и создание структуры для отображения в TreeView
        /// </summary>
        /// <param name="entityParam">Цельная строка всех параметров сущности</param>
        /// <returns>Список, содержащий списки параметров</returns>
        private List<List<EntityParam>> parseParams(EntityParam entityParam)
        {
            //Регулярное выражение для поиска скобок и отделения вложенных параметров (тех что в скобках)
            Regex regexBrackets = new Regex(PATTERN_PRIMARY_PARAMS_WITH_BRACKETS);
            //Регулярное выражение для поиска запятых и разделения по ним на отдельные параметры
            Regex regexCommas = new Regex(PATTERN_SECONDARY_PARAMS_WITH_COMMAS);
            //Список, в котором будут содержаться все параметры сущности
            List<List<EntityParam>> resultParams = new List<List<EntityParam>>();
            //Временный список для тех параметров, которые были в скобках (они вырезаются и заменяются сгенерированным именем)
            List<EntityParam> replacedParams = new List<EntityParam>();

            //Выделение параметров в скобках, если такие есть
            MatchCollection matches = regexBrackets.Matches(entityParam.value);
            if (matches.Count > 0)
            {
                //Копирование строки параметров для того, чтобы не нарушить её исходное содержание
                String resultParamsForEntity = entityParam.value.Substring(0, entityParam.value.Length);

                //Проход по результатам работы регулярного выражения
                foreach (Match item in matches)
                {
                    //Генерирование имени для замены: узнаваемая константа + системное время для уникальности
                    String replacedName = REPLACEMENT_NAME_PART + (Environment.TickCount.ToString());

                    //Удаление из строки параметров параметра (набора параметров) со скобками
                    resultParamsForEntity = resultParamsForEntity.Remove(item.Index, item.Length);
                    //Вставка на место параметра (набора параметров) со скобками сгенерированного имени
                    resultParamsForEntity = resultParamsForEntity.Insert(item.Index, replacedName);
                    //Создание объекта для вырезанного параметра (набора параметров) в скобках
                    EntityParam newParam = new EntityParam(item.Groups[1].Value, replacedName);
                    //Сохранение объекта параметра во временном списке для дальнейшего разбора
                    replacedParams.Add(newParam);

                    //place for recursive parse enclosured brackets if needed
                }
                //Присвоение строке параметров сущности строки с замененными параметрами в скобках, т.е. строки без скобок
                entityParam.value = resultParamsForEntity;
            }

            //Добавление в конец строки параметров символа "," для корректной работы регулярного выражения
            entityParam.value = entityParam.value + ",";

            //Применение регулярного выражения для выделения параметров между запятыми
            matches = regexCommas.Matches(entityParam.value);
            if (matches.Count > 0)
            {
                //Проход по результатам работы регулярного выражения
                foreach (Match item in matches)
                {
                    //Списоок параметров
                    List<EntityParam> primaryParamsBranch = new List<EntityParam>();

                    //Создание временного параметра для исследования на предмет того, является ли он одичным параметром или замененным набором параметров
                    EntityParam tempParam = new EntityParam(item.Groups[1].Value, item.Groups[1].Value);
                    //Проверка, есть ли рассматриваемый параметр в списке замененных (только по имени)
                    if (replacedParams.Count > 0 && replacedParams.Contains(tempParam))
                    {
                        //Если параметр в списке по имени найден, то происходит получение его номера в списке
                        int index = replacedParams.IndexOf(tempParam);
                        //Переопределение параметра (получение взамен имени списка параметров)
                        tempParam = replacedParams[index];

                        //Применение регулярного выражения для выделения вложенных параметров
                        MatchCollection insertedMatches = regexCommas.Matches(tempParam.value + ",");
                        if (insertedMatches.Count > 0)
                        {
                            //Проход по результатам работы регулярного выражения
                            foreach (Match insertedItem in insertedMatches)
                            {
                                //Добавление в список параметров всех параметров в отдельности
                                primaryParamsBranch.Add(new EntityParam(insertedItem.Groups[1].Value, insertedItem.Groups[1].Value));
                            }
                        }
                    }
                    else
                    {
                        //Если параметр не является замененным, то он сразу добавляется в список параметров - он будет в этом списке единственным
                        primaryParamsBranch.Add(new EntityParam(item.Groups[1].Value, item.Groups[1].Value));
                    }

                    //Добавление в результирующий список списков, содержащих единичные параметры либо наборы параметров
                    resultParams.Add(primaryParamsBranch);
                }
            }

            return resultParams;
        }

    }
}
