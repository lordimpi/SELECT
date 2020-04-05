CREATE OR REPLACE PACKAGE MOTO_PACKAGE 
IS
    PROCEDURE P_CONSULTARMOTOS(o_cursor IN OUT SYS_REFCURSOR,V_COL IN VARCHAR2);
    PROCEDURE P_CONSULTARMOTOSWHERE(o_cursor IN OUT SYS_REFCURSOR, V_COL IN VARCHAR2,V_COND IN VARCHAR2);
    PROCEDURE P_BUSCARTABLA(V_NOMBRETABLA in VARCHAR2, V_EXISTE OUT VARCHAR2);
    PROCEDURE P_CANTIDADFILAS(V_NOMBRETABLA IN VARCHAR2, V_CANTIDAD OUT NUMBER);
    PROCEDURE P_CONSULTADICCIONARIO(o_cursor IN OUT SYS_REFCURSOR, V_COL IN VARCHAR2);

END MOTO_PACKAGE;

CREATE OR REPLACE PACKAGE BODY MOTO_PACKAGE IS

    PROCEDURE P_CONSULTARMOTOS(o_cursor IN OUT SYS_REFCURSOR,V_COL IN VARCHAR2)
    IS
    BEGIN
        OPEN o_cursor FOR
        'SELECT '||V_COL||' FROM MOTO';
    END P_CONSULTARMOTOS;
    
    PROCEDURE P_CONSULTARMOTOSWHERE(o_cursor IN OUT SYS_REFCURSOR, V_COL IN VARCHAR2,V_COND IN VARCHAR2)
    IS
    BEGIN
        OPEN o_cursor FOR
            'SELECT '||V_COL||' FROM MOTO '||V_COND;
    END P_CONSULTARMOTOSWHERE;

    PROCEDURE P_CONSULTADICCIONARIO(o_cursor IN OUT SYS_REFCURSOR, V_COL IN VARCHAR2)
    IS
    BEGIN
        OPEN o_cursor FOR
        'SELECT '||V_COL|| ' FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = ''MOTO''';
    END P_CONSULTADICCIONARIO;

    PROCEDURE P_BUSCARTABLA(V_NOMBRETABLA in VARCHAR2, V_EXISTE OUT varchar2)
    IS
        NombreTabla VARCHAR2(256) := '';
    BEGIN
        SELECT TABLE_NAME
        INTO NombreTabla
        FROM USER_TABLES
        WHERE TABLE_NAME = UPPER(V_NOMBRETABLA);
        IF NombreTabla = UPPER(V_NOMBRETABLA) THEN
            v_existe := 'SI';
        ELSE
            V_EXISTE := 'NO';
        END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                v_existe := 'NO';
    END P_BUSCARTABLA;
    
    PROCEDURE P_CANTIDADFILAS(V_NOMBRETABLA IN VARCHAR2, V_CANTIDAD OUT NUMBER)
    IS 
    BEGIN
        FOR I IN (SELECT TABLE_NAME FROM USER_TABLES where TABLE_NAME = V_NOMBRETABLA) LOOP
        EXECUTE IMMEDIATE 'SELECT count(*) FROM ' || i.table_name INTO V_CANTIDAD;
        END LOOP;
    END P_CANTIDADFILAS;

END MOTO_PACKAGE;