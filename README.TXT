A junior developer was tasked with writing a reusable implementation for a mass mailing application (the 
AddressProcessing project) to read and write text files that hold tab separated data. They proceeded and 
as a result produced the CSVReaderWriter class which can be found in the "Exercise" solution in the 
"AddressProcessing.CSV" namespace.
The implementation, although it works and meets the needs of the application, is of very low quality.

Your task:
	 - Identify and annotate the shortcomings in the current implementation as if you were doing a code 
	   review, using comments in the CSVReaderWriterForAnnotation.cs file.
	 - Refactor the CSVReaderWriter implementation into clean, elegant, rock-solid & well performing 
	   code, without over-engineering. You are not required to update the AddressFileProcessor as part of
	   this task.
	 - Where you make trade offs, comment & explain why.
	 - Assume this code is in production and backwards compatibility must be maintained. Therefore if you 
	   decide to change the public interface, please deprecate the existing methods. Feel free to evolve 
	   the code in other ways though (hint: unit tests will help here).
	 - You may introduce nuget based references to mocking or testing frameworks but do not include postsharp 
		   or other proprietary dependencies.
	



!!! THE RESULT NOTES IS IN "CSVReaderWriter For Annotation.cs"






