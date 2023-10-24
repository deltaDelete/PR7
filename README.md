# Проект 2
## Предметная область

> [!NOTE]
> Медицина и здравоохранение: Создание модулей для управления медицинскими записями пациентов, расписанием приемов, лекарственными препаратами, медицинской диагностикой и анализом данных.

## ER-диаграмма

<!-- TODO Упорядочить таблицы -->

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
    }
    Doctor {
        int Id PK
        string Name
        string Specialty
        string Phone
        string Email
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
    }

    DiagnosisType {
        int DiagnosisTypeId PK
        string DiagnosisName
    }

    Patient       ||--|{ Appointment : "есть"
    Doctor        ||--|{ Appointment : "проводит"
    Patient       ||--|{ Medication  : "принимает"
    Patient       ||--|{ Diagnosis   : "получает"
    Doctor        ||--|{ Diagnosis   : "делает"
    DiagnosisType ||--|{ Diagnosis   : "имеет"
    Drug          ||--|{ Medication  : "какой препарат"
```
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

## Расчет загруженности
> [!NOTE]
> Когда на неделе больше всего и меньше всего пациентов записано.
