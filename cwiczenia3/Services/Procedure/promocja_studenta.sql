USE [s15035]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[promote_students]
	@Studies VARCHAR(50),
	@Semester int,
	@Semester_out int OUTPUT,
	@Id_enrollment_out int OUTPUT,
	@Start_date_out date OUTPUT
AS
DECLARE
    @study_id int,
	@enrollment_id int,
	@new_enrollement_id int,
	@max_enrollment_id int
BEGIN
	SELECT @study_id = IdStudy FROM Studies WHERE Name = @Studies;
	SELECT @enrollment_id = IdEnrollment FROM Enrollment WHERE Semester = @Semester AND IdStudy = @study_id;
	IF @enrollment_id IS NUll Raiserror('Brak wpisow na podany semestr', 11, 20)
	SELECT @new_enrollement_id = IdEnrollment FROM Enrollment WHERE Semester = (@Semester + 1) AND IdStudy = @study_id;
	IF @new_enrollement_id IS NULL
		BEGIN
			SELECT @max_enrollment_id = max(IdEnrollment) FROM Enrollment;
			SET @new_enrollement_id = @max_enrollment_id + 1;
			INSERT INTO Enrollment VALUES(@new_enrollement_id, @Semester + 1, @study_id, GETDATE());
		END;
	UPDATE Student SET IdEnrollment = @new_enrollement_id WHERE IdEnrollment = @enrollment_id;
	SELECT @Semester_out = Semester, @Id_enrollment_out = IdEnrollment, @Start_date_out = StartDate FROM Enrollment WHERE IdEnrollment = @new_enrollement_id;
END; 