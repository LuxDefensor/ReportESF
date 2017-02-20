WITH AllPoints(PointID, PointName, PointType, ID_Parent) 
	AS (
	SELECT ID_Point, PointName, Point_Type, ID_Parent
	FROM Points 
	WHERE ID_Point = 1
	UNION ALL 
	SELECT Points.ID_Point, Points.PointName, Points.Point_Type, Points.ID_Parent 
	FROM Points INNER JOIN AllPoints ON Points.ID_Parent = AllPoints.PointID
	)
	SELECT ap.PointID,ap.ID_Parent,ap.PointName,
	ap.PointType
	FROM AllPoints ap 
	where ap.PointType in (8,7,151,148,147,10,5,1,2)

	select pp.ID_PP,t.ParamName
	from pointparams pp
	inner join PointParamTypes t on pp.ID_Param=t.ID_Param
	where ID_Point=16821
	and pp.ID_Param in (2,4,6,8)
