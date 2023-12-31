# Проект 2
## Предметная область

> [!NOTE]
> Медицина и здравоохранение: Создание модулей для управления медицинскими записями пациентов, расписанием приемов, лекарственными препаратами, медицинской диагностикой и анализом данных.

## ER-диаграмма

```mermaid
erDiagram
    Patient {
        int PatientId PK
        string FullName
        string Address
        string Phone
        string Email
    }
    Appointment {
        int AppointmentId PK
        int PatientId FK
        int DoctorId FK
        datetime AppointmentDate
        string AppointmentType
        string Comment
    }
    Doctor {
        int Id PK
        string Name
        int SpecialtyId FK
        string Phone
        string Email
    }

    Speciality {
        int SpecialityId PK
        string SpecialityName
    }

    Medication {
        int MedicationId PK
        int DrugId FK
        decimal Dosage
        string Frequency
        int PatientId FK
    }

    Drug {
        int DrugId PK
        string DrugName
    }

    Diagnosis {
        int DiagnosisId PK
        int PatientId FK
        int DoctorId FK
        datetime DiagnosisDate
        int DiagnosisType FK
        string Comment
    }

    DiagnosisType {
        int DiagnosisTypeId PK
        string DiagnosisName
    }

    Patient       ||--|{ Appointment : "есть"
    Doctor        ||--|{ Appointment : "проводит"
    Speciality    ||--|{ Doctor      : "имеет"
    Patient       ||--|{ Medication  : "принимает"
    Patient       ||--|{ Diagnosis   : "получает"
    Doctor        ||--|{ Diagnosis   : "делает"
    DiagnosisType ||--|{ Diagnosis   : "имеет"
    Drug          ||--|{ Medication  : "какой препарат"
```
[Диаграмма в другой утилите](https://dbdiagram.io/d/PR7-65375bc7ffbf5169f050827c)
## Блок-схемы
###  Расчет количества назначенных препаратов
> [!NOTE]
> Сколько определенного препарата было назначено и какая средняя дозировка этого препарата
```mermaid
flowchart TD

START([Начало]) --> 
1["Выполнить запрос
select  Drug.DrugName as `Наименование препарата`,
        sum(Medication.Dosage) as `Всего прописано всем`,
        avg(Medication.Dosage) as `Средняя дозировка`
from Medication 
join Drug on Drug.DrugId = Medication.DrugId
group by Medication.DrugId"]
--> 2[[Замаппить результат запроса на таблицу]]
--> 3[/Вывести таблицу/]
--> END([Конец])
```

### Расчет загруженности
> [!NOTE]
> Когда на неделе больше всего и меньше всего пациентов записано.

## [Спецификация требований](REQSPEC.md)
