AP = A1 (A1) =text> A1 | Pa1 (Pa1) =text> Pa1
Del = "," =text> "," | "-" =text> "-"
NE = "��" =text> "��"
WD = W1 =text> W1 | Del1 =text> Del1
PG = "," [NE1] Pa1 {WD1} "," (Pa1) =text> "," NE1 Pa1 WD1 ","
MSP = N1 (N1) =text> N1 | N1 N2<c=gen> (N1) =text> N1 N2<c=gen> | AP1 N1 <AP1=N1> (N1) =text> AP1 N1 <AP1~>N1> | AP1 AP2 N1 <AP1=AP2=N1> (N1) =text> AP1 AP2 N1 <AP1~>N1, AP2~>N1> | AP1 N1 N2<c=gen> <AP1=N1> (N1) =text> AP1 N1<AP1~>N1> N2<c=gen>| N1 AP1 N2<c=gen> <AP1=N2> (N1) =text> N1 AP1 N2<c=gen><AP1~>N2> | N1 N2<c=gen> N3<c=gen> (N1) =text> N1 N2<c=gen> N3<c=gen>
Prep = Pr1 MSP1 (MSP1) =text> Pr1 MSP1 <Pr1~>MSP1>

NMSPN = N1 (N1) =text> N1 
TANSyn = AP1 ["\("AP2"\)"]  <AP1=AP2> (AP1) =text> AP1
TAN = TANSyn1 [[","] "���" TANSyn2 [","]] <TANSyn1=TANSyn2> (TANSyn1) =text> TANSyn1
TNN = NMSPN1 [PG1] <NMSPN1=PG1> (NMSPN1) =text> NMSPN1

SDefNA = "��������" Pn1<�����> TNN1<c=acc> TAN1<c=ins> <Pn1=TAN1,TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNB = TNN1<c=nom> ["������"|"�"] V1<����������, t=pres, p=3, m=ind> TAN1<c=ins> <TNN1.g=TAN1.g, TNN1.a=TAN1.a, TNN1.n=V1.n=TAN1.n> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNC = TNN1<c=acc> ["�����"] "��������" TAN1<c=ins> <TNN1.g=TAN1.g, TNN1.a=TAN1.a, TNN1.n=TAN1.n>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefND = "�������" TNN1<c=acc> [Prep1<c=prep>] TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNE = TNN1<c=acc> "�������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNF = TNN1<c=nom> ["�������"] "�������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNG = TNN1<c=acc> ["�����"] "�����" "��������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNH = TNN1<c=acc> "��" "��������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNI = TNN1"," ["�������"|"������"] Pa1<�������> TAN1<c=ins> <TNN1.g=TAN1.g, TNN1.a=TAN1.a, TNN1.n=V1.n=TAN1.n> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefNJ = TNN1<c=acc> [","{W}","] "�����" "�������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a> =text> TAN1 #TNN1 <TAN1~>TNN1>