USE [dev.ea.com]
GO
/****** Object:  StoredProcedure [dbo].[DNNrocket_GetList]    Script Date: 04/01/2019 15:23:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER    PROCEDURE [dbo].[DNNrocket_GetList]
@PortalId int, 
@ModuleId int,
@TypeCode nvarchar(50),
@Filter nvarchar(1000),
@OrderBy nvarchar(500),
@ReturnLimit int = 0,
@pageNum int = 0,
@PageSize int = 0,
@RecordCount int = 0,
@Lang nvarchar(50) = ''

AS
begin

------------------------------------------------------------------------------------
--- Cursor for INDEX records.
------------------------------------------------------------------------------------
DECLARE @idxname nvarchar(250) = ''
DECLARE @idxparentItemId int = 0
DECLARE @systemlinkItemId int = 0
DECLARE @JoinIndex nvarchar(max)

select @systemlinkItemId = itemid from dbo.[DNNrocket] where [TypeCode] = 'SYSTEMLINK' and GUIDkey = @TypeCode 

DECLARE idx_cursor CURSOR FOR 
SELECT GUIDKey, ParentItemId
FROM dbo.[DNNrocket]
WHERE [TypeCode] = 'SYSTEMLINKIDX' and ParentItemId = @systemlinkItemId

SET @JoinIndex = ''

OPEN idx_cursor  
FETCH NEXT FROM idx_cursor INTO @idxname,@idxparentItemId

WHILE @@FETCH_STATUS = 0  
BEGIN  

	set @JoinIndex = @JoinIndex + ' left join dbo.[DNNrocket] as [' + @idxname + '] on [' + @idxname + '].ParentItemId = R1.ItemId and [' + @idxname + '].TypeCode = ''' + @TypeCode  + '_' + @idxname + ''' '

	FETCH NEXT FROM idx_cursor INTO @idxname,@idxparentItemId
END 

CLOSE idx_cursor
DEALLOCATE idx_cursor
;  
print @JoinIndex
------------------------------------------------------------------------------------
------------------------------------------------------------------------------------




	SET NOCOUNT ON
	  DECLARE
		 @STMT nvarchar(max)         -- SQL to execute
		,@rtnFields nvarchar(max)

	IF (@PortalId >= 0) BEGIN

		IF (@ModuleId >= 0) BEGIN
			SET @Filter = ' and (R1.PortalId = '''  + Convert(nvarchar(10),@PortalId) + ''' or R1.PortalId = ''-1'') and (R1.ModuleId = ''' + Convert(nvarchar(10),@ModuleId) + ''' or R1.ModuleId = ''-1'') ' + @Filter
		END ELSE BEGIN
			SET @Filter = ' and (R1.PortalId = '''  + Convert(nvarchar(10),@PortalId) + '''  or R1.PortalId = ''-1'') ' + @Filter
		END 

	END 

	SET @Filter = REPLACE(@Filter,'[XMLData]','dbo.[DNNrocketLangMerge](RLang1.[XMLData],R1.[XMLData])')
	SET @OrderBy = REPLACE(@OrderBy,'[XMLData]','dbo.[DNNrocketLangMerge](RLang1.[XMLData],R1.[XMLData])')

	set @rtnFields = ' R1.[ItemId] '
	set @rtnFields = @rtnFields + ',dbo.[DNNrocketLangMerge](RLang1.[XMLData],R1.[XMLData]) as [XMLData] '				
	set @rtnFields = @rtnFields + ',ISNULL(RLang1.[Lang],ISNULL(R1.[Lang],'''')) as [Lang] '	 
 
	set @rtnFields = @rtnFields + ',R1.[PortalId] '
	set @rtnFields = @rtnFields + ',R1.[ModuleId] '
	set @rtnFields = @rtnFields + ',R1.[TypeCode] '
	set @rtnFields = @rtnFields + ',R1.[GUIDKey] '
	set @rtnFields = @rtnFields + ',R1.[ModifiedDate] '
	set @rtnFields = @rtnFields + ',R1.[TextData] '
	set @rtnFields = @rtnFields + ',R1.[XrefItemId] '
	set @rtnFields = @rtnFields + ',R1.[ParentItemId] '
	set @rtnFields = @rtnFields + ',R1.[UserId] '


	IF (@PageSize > 0)
	BEGIN
			-- Do Paging
		SET @STMT = 'DECLARE @recct int '
		set @STMT = @STMT + ' SET @recct = ' + Convert(nvarchar(5),@RecordCount) 
		
		set @STMT = @STMT + '   DECLARE @lbound int, @ubound int '

		SET @pageNum = ABS(@pageNum)
		SET @pageSize = ABS(@pageSize)
		IF @pageNum < 1 SET @pageNum = 1
		IF @pageSize < 1 SET @pageSize = 1

		set @STMT = @STMT + ' SET @lbound = ' + convert(nvarchar(50),((@pageNum - 1) * @pageSize))
		set @STMT = @STMT + ' SET @ubound = @lbound + ' + convert(nvarchar(50),(@pageSize + 1))
		set @STMT = @STMT + ' IF @lbound >= @recct BEGIN '
		set @STMT = @STMT + '   SET @ubound = @recct + 1 '
		set @STMT = @STMT + '   SET @lbound = @ubound - (' + convert(nvarchar(50),(@pageSize + 1)) + ') ' -- return the last page of records if no records would be on the specified page '
		set @STMT = @STMT + ' END '
		
		-- Default order by clause
		if @OrderBy = '' 
		Begin
			set @OrderBy = ' ORDER BY R1.ModifiedDate DESC '
		End
		
		set @STMT = @STMT + ' SELECT '
		if @ReturnLimit > 0 
		begin
			set @STMT = @STMT + ' top ' + convert(nvarchar(10),@ReturnLimit)
		end
		
		set @STMT = @STMT + @rtnFields		

		set @STMT = @STMT + ' FROM    (
								SELECT  ROW_NUMBER() OVER(' + @orderBy + ') AS row, '
		set @STMT = @STMT + @rtnFields		
		set @STMT = @STMT + ' FROM dbo.[DNNrocket]  as R1 '
		set @STMT = @STMT + ' left join dbo.[DNNrocket] as RLang1 on RLang1.ParentItemId = R1.ItemId and RLang1.[Lang] = ''' + @Lang + '''  and RLang1.TypeCode =  R1.TypeCode + ''LANG'' ' 
		
				IF (RIGHT(@TypeCode,1) = '%')
			BEGIN
				set @STMT = @STMT + 'WHERE R1.TypeCode Like ''' + @TypeCode + ''' ' + @Filter  
			END ELSE
			BEGIN
				IF (@TypeCode = '')
				BEGIN
					set @STMT = @STMT + 'WHERE R1.TypeCode != ''''' + @Filter  
				END ELSE
				BEGIN
					set @STMT = @STMT + 'WHERE R1.TypeCode = ''' + @TypeCode + ''' ' + @Filter  
				END
			END	                                                              
			
			set @STMT = @STMT + ' ) AS R1 '
			set @STMT = @STMT + ' left join dbo.[DNNrocket] as RLang1 on RLang1.ParentItemId = R1.ItemId and RLang1.[Lang] = ''' + @Lang + '''  and RLang1.TypeCode =  R1.TypeCode + ''LANG'' ' 
			set @STMT = @STMT + ' WHERE row > @lbound AND row < @ubound '

			
	END ELSE
	BEGIN
		-- DO NON-PAGING

		set @STMT = ' SELECT '
		if @ReturnLimit > 0 
		begin
			set @STMT = @STMT + ' top ' + convert(nvarchar(10),@ReturnLimit)
		end
		
		set @STMT = @STMT + @rtnFields		

		set @STMT = @STMT + ' FROM dbo.[DNNrocket]  as R1 '
		set @STMT = @STMT + ' left join dbo.[DNNrocket] as RLang1 on RLang1.ParentItemId = R1.ItemId and RLang1.[Lang] = ''' + @Lang + '''  and RLang1.TypeCode =  R1.TypeCode + ''LANG'' ' 
	
		IF (RIGHT(@TypeCode,1) = '%')
		BEGIN
			set @STMT = @STMT + 'WHERE R1.TypeCode Like ''' + @TypeCode + ''' ' + @Filter  
		END ELSE
		BEGIN
			IF (@TypeCode = '')
			BEGIN
				set @STMT = @STMT + 'WHERE R1.TypeCode != ''''' + @Filter  
			END ELSE
			BEGIN
				set @STMT = @STMT + 'WHERE R1.TypeCode = ''' + @TypeCode + ''' ' + @Filter  
			END
		END	                                                              	

		set @STMT = @STMT + ' ' + @OrderBy                                                           	

	END		


	print @STMT
	EXEC sp_executeSQL @STMT                 -- return requested records

end