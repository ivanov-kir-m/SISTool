AP = A1 (A1) =text> A1 | Pa1 (Pa1) =text> Pa1
NE = "��" =text> "��"
Del = "," =text> "," | "-" =text> "-"
PMSP = N1 (N1) =text> N1 | AP1 N1 <AP1=N1> (N1) =text> AP1 N1 | N1 N2<c=gen> (N1) =text> N1 N2<c=gen>
WD = W1 =text> W1 | Del1 =text> Del1
SG = "," [Pr1] Pn1<�������> {WD1} "," (Pn1) =text> "," Pr1 Pn1 WD1 ","
PG = "," [NE1] Pa1 {WD1} "," (Pa1) =text> "," NE1 Pa1 WD1 ","
NMSP = N1 (N1) =text> N1 | N1 N2<c=gen> (N1) =text> N1 N2<c=gen>
NMSPTWO = N1 (N1) =text> "["N1"]" ";" | N1 N2<c=gen> (N1) =text> "["N1"]" N2<c=gen> ";"
MSP = N1 (N1) =text> N1 | N1 N2<c=gen> (N1) =text> N1 N2<c=gen> | AP1 N1 <AP1=N1> (N1) =text> AP1 N1 <AP1~>N1> | AP1 AP2 N1 <AP1=AP2=N1> (N1) =text> AP1 AP2 N1 <AP1~>N1, AP2~>N1> | AP1 N1 N2<c=gen> <AP1=N1> (N1) =text> AP1 N1<AP1~>N1> N2<c=gen>| N1 AP1 N2<c=gen> <AP1=N2> (N1) =text> N1 AP1 N2<c=gen><AP1~>N2> | N1 N2<c=gen> N3<c=gen> (N1) =text> N1 N2<c=gen> N3<c=gen>
Prep = Pr1 MSP1 (MSP1) =text> Pr1 MSP1 <Pr1~>MSP1>
APXXX = ',' AP1 {W} (AP1) =text> ',' AP1 W
Def = MSP1 (MSP1) =text> MSP1 | '��' =text> '��' | '���' =text> '���' | Pn1<�����> MSP1 <Pn1=MSP1> (MSP1) =text> Pn1 MSP1<Pn1~>MSP1> | Pn1<�����> MSP1 <Pn1=MSP1> (MSP1) =text> Pn1 MSP1<Pn1~>MSP1> | Pn1<�����> MSP1 <Pn1=MSP1> (MSP1) =text> Pn1 MSP1<Pn1~>MSP1> | Pn1<������> MSP1 <Pn1=MSP1> (MSP1) =text> Pn1 MSP1<Pn1~>MSP1> | Pn1<���> MSP1 <Pn1=MSP1> (MSP1) =text> Pn1 MSP1<Pn1~>MSP1> | MSP1 PG1 <MSP1=PG1> (MSP1) =text> MSP1 PG1 <MSP1~>PG1> | MSP1 SG1 <MSP1=SG1> (MSP1) =text> MSP1 SG1 <MSP1~>SG1>
DefIns = MSP1 Pr1<�> N1<c=ins> (MSP1) =text> MSP1 Pr1 N1<c=ins> | MSP1 Pr1<���> N1<c=gen> (MSP1) =text> MSP1 Pr1 N1<c=gen>
DefXXX = Def1 (Def1) =text> Def1 | Def1 APXXX1 (Def1) =text> Def1 APXXX1
TermSyn = MSP1 ["\("MSP2"\)"] <MSP1.c=MSP2.c> (MSP1) =text> MSP1
TermASyn = AP1 ["\("AP2"\)"]  <AP1=AP2> (AP1) =text> AP1
TermN = NMSP1 [PG1] <NMSP1=PG1> (NMSP1) =text> NMSP1
Term = TermSyn1 [[","] "���" ["������"] TermSyn2] <TermSyn1.c=TermSyn2.c> (TermSyn1) =text> TermSyn1
TermA = TermASyn1 [[","] "���" TermASyn2 [","]] <TermASyn1=TermASyn2> (TermASyn1) =text> TermASyn1

NMSPN = N1 (N1) =text> N1 
MSPN = N1 (N1) =text> N1
NMSPTWON = N1 (N1) =text> N1
TSN = MSPN1 ["\("MSPN2"\)"] <MSPN1.c=MSPN2.c> (MSPN1) =text> MSPN1
TN = TSN1 [[","] "���" ["������"] TSN2] <TSN1.c=TSN2.c> (TSN1) =text> TSN1
TANSyn = AP1 ["\("AP2"\)"]  <AP1=AP2> (AP1) =text> AP1
TAN = TANSyn1 [[","] "���" TANSyn2 [","]] <TANSyn1=TANSyn2> (TANSyn1) =text> TANSyn1
TNN = NMSPN1 [PG1] <NMSPN1=PG1> (NMSPN1) =text> NMSPN1

DefN = TN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TN1
DefN = '���' TN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TN1
DefN = TN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TN1
DefN = TN1<c=ins> '��' '��������' Defs1<c=acc> =text> #TN1
DefN = '���' TN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TN1
DefN = TN1<c=ins> "��" "��������" Def1<c=acc> =text> #TN1
DefN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TN1
DefN = TN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TN1
DefN = Def1<c=acc> ["��"] "�����" "��������" TN1<c=ins>  =text> #TN1
DefN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TN1
DefN = Def1<c=acc> "�����" "��������" TN1<c=ins>  =text> #TN1
DefN = Def1<c=acc> ["������"] "�������" TN1<c=ins> =text> #TN1
DefN = "�������" TN1<c=ins> Def1<c=acc>  =text> #TN1
DefN = "�������" Def1<c=acc> TN1<c=ins> =text> #TN1
DefN = "�������" Def1<c=acc> TN1<c=nom> =text> #TN1
DefN = Def1<c=acc> "," Pn1<�������> "�������" TN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TN1
DefN = Def1<c=acc> "�����" "�������" TN1<c=ins> =text> #TN1
DefN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TN1
DefN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TN1<c=ins> =text> #TN1
DefN = "�����" "����" Pa1<�������, f=short> TN1<c=ins> <Pa1.n=TN1.n> =text> #TN1
DefN = TN1<c=ins> ["\("MSPN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TN1
DefN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TN1<c=ins> <Def1.n=V1.n> =text> #TN1
DefN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TN1<c=ins> <V1.n=Def1.n> =text> TN1
DefN = Def1<c=nom> "����������" ["�����" "���������" "-"] TN1<c=nom> =text> #TN1
DefN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TN1
DefN = TN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TN1
DefN = Def1<c=acc> "�������" "��������" TN1<c=ins> =text> #TN1
DefN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TN1<c=ins> =text> #TN1
DefN = TN1<c=ins> "��������" Def1<c=acc> =text> #TN1
DefN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TN1<c=ins> <Def1=Pa1> =text> #TN1
DefN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TN1<c=nom> <Pa1.n=N1.n> =text> #TN1
DefN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TN1
DefN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TN1<c=nom> <Def1.n=V1.n> =text> #TN1
DefN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TN1<c=gen> <Def1.n=V1.n> =text> #TN1
DefN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TN1<c=nom> <Def1.n=V1.n> =text> #TN1
DefN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TN1
DefN = "���" Pa1<��������> TN1 <Pa1=TN1> =text> #TN1
DefN = {"�.�." | "�" "." "�" "."}<1,1> TN1 =text> #TN1
DefN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TN1<c=ins> <Pa1=Def1>  =text> #TN1
DefN = Def1 "," Pa1<��������> TN1<c=nom> <Pa1=Def1> =text> #TN1
DefN = Def1 { "," | "\(" }<1,1> Pa1<�������> TN1<c=ins> <Def1=Pa1> =text> #TN1
DefN = Pn1 V1<��������, t=pres, p=3, m=ind> TN1<c=ins> <Pn1=V1> =text> #TN1
DefN = "���" TN1<c=ins> "��" "��������" Def1<c=acc> =text> #TN1
DefN = "���" TN1<c=ins> "��������" Def1<c=acc> =text> #TN1
DefN = "���" TN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TN1
DefN = "�����" "��������" "���" TN1<c=ins> Def1<c=acc> =text> #TN1
DefN = "������" "�������" TN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TN1
DefN = "������" "�������" TN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TN1
DefN = "���" "��������" TN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TN1
DefN = "���" TN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TN1
DefN = "���" TN1<c=ins> "��" "��������" Def1<c=acc> =text> #TN1
DefN = "���" TN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TN1
DefN = "���" "��������" TN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TN1.n=V1.n> =text> #TN1
DefN = N1<������> TN1<c=nom> =text> #TN1
DefN = "���" "������" TN1<c=nom> "����������" Def1<c=nom> =text> #TN1
DefN = TN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TN1
DefN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TN1<c=gen> =text> #TN1
DefN = TN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TN1
DefN = TN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TN1.c=Def1.c> =text> #TN1
DefN = Pr1 TN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TN1.c=Def1.c> =text> #TN1
DefN = AP1 "\(" {"�.�."|"��" "����"|"�." "�."}<1,1> {W1} "\)" NMSPTWON1 <AP1=NMSPTWON1> =text> #NMSPTWON1

SDefNA = "��������" Pn1<�����> TNN1<c=acc> TAN1<c=ins> <Pn1=TAN1,TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1<c=nom> ["������"|"�"] V1<����������, t=pres, p=3, m=ind> TAN1<c=ins> <TNN1.g=TAN1.g, TNN1.a=TAN1.a, TNN1.n=V1.n=TAN1.n> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1<c=acc> ["�����"] "��������" TAN1<c=ins> <TNN1.g=TAN1.g, TNN1.a=TAN1.a, TNN1.n=TAN1.n>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = "�������" TNN1<c=acc> [Prep1<c=prep>] TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a> =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1<c=acc> "�������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1<c=nom> ["�������"] "�������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1<c=acc> ["�����"] "�����" "��������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1<c=acc> "��" "��������" TAN1<c=ins> <TAN1.n=TNN1.n, TAN1.g=TNN1.g, TNN1.a=TAN1.a>  =text> TAN1 #TNN1 <TAN1~>TNN1>
SDefN = TNN1"," ["�������"|"������"] Pa1<�������> TAN1<c=ins> <TNN1.g=TAN1.g, TNN1.a=TAN1.a, TNN1.n=V1.n=TAN1.n> =text> TAN1 #TNN1 <TAN1~>TNN1>

NPNMSPN = N1 (N1) =text> "N1[" #N1 "] =text] #N1"
NPMSPN = N1 (N1) =text> "N1[" #N1 "] (N1) =text] #N1"
NPNMSPTWON = N1 (N1) =text> "N1[" #N1 "] =text] #N1"

NPTANSyn = AP1 ["\("AP2"\)"]  <AP1=AP2> (AP1) =text> AP1
NPTNSyn = NPMSPN1 ["\("NPMSPN2"\)"] <NPMSPN1.c=NPMSPN2.c> (NPMSPN1) =text> NPMSPN1
NPTN = NPTNSyn1 [[","] "���" ["������"] NPTNSyn2] <NPTNSyn1.c=NPTNSyn2.c> (NPTNSyn1) =text> NPTNSyn1

NPTNN = NPNMSPN1 [PG1] <NPNMSPN1=PG1> (NPNMSPN1) =text> NPNMSPN1
NPTAN = NPTANSyn1 [[","] "���" NPTANSyn2 [","]] <NPTANSyn1=NPTANSyn2> (NPTANSyn1) =text> NPTANSyn1

NPDefN = NPTN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTN1 
NPDefN = '���' NPTN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTN1 
NPDefN = NPTN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTN1 
NPDefN = NPTN1 <c=ins> '��' '��������' DefIns1<c=acc> =text> #NPTN1 
NPDefN = '���' NPTN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTN1 
NPDefN = NPTN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTN1 
NPDefN = NPTN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTN1 
NPDefN = Def1<c=acc> ["��"] "�����" "��������" NPTN1 <c=ins>  =text> #NPTN1 
NPDefN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTN1 
NPDefN = Def1<c=acc> "�����" "��������" NPTN1 <c=ins>  =text> #NPTN1 
NPDefN = Def1<c=acc> ["������"] "�������" NPTN1 <c=ins> =text> #NPTN1 
NPDefN = "�������" NPTN1 <c=ins> Def1<c=acc>  =text> #NPTN1 
NPDefN = "�������" Def1<c=acc> NPTN1 <c=ins> =text> #NPTN1 
NPDefN = "�������" Def1<c=acc> NPTN1 <c=nom> =text> #NPTN1 
NPDefN = Def1<c=acc> "," Pn1<�������> "�������" NPTN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTN1 
NPDefN = Def1<c=acc> "�����" "�������" NPTN1 <c=ins> =text> #NPTN1 
NPDefN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTN1 
NPDefN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTN1 <c=ins> =text> #NPTN1 
NPDefN = "�����" "����" Pa1<�������, f=short> NPTN1 <c=ins> <Pa1.n=NPTN1 .n> =text> #NPTN1 
NPDefN = NPTN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTN1 
NPDefN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTN1 <c=ins> <Def1.n=V1.n> =text> #NPTN1 
NPDefN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTN1 <c=ins> <V1.n=Def1.n> =text> #NPTN1 
NPDefN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTN1 <c=nom> =text> #NPTN1 
NPDefN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTN1 
NPDefN = NPTN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = Def1<c=acc> "�������" "��������" NPTN1 <c=ins> =text> #NPTN1 
NPDefN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTN1 <c=ins> =text> #NPTN1 
NPDefN = NPTN1 <c=ins> "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTN1 <c=ins> <Def1=Pa1> =text> #NPTN1 
NPDefN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTN1 <c=nom> <Pa1.n=N1.n> =text> #NPTN1 
NPDefN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTN1 
NPDefN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTN1 <c=nom> <Def1.n=V1.n> =text> #NPTN1 
NPDefN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTN1 <c=gen> <Def1.n=V1.n> =text> #NPTN1 
NPDefN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTN1 <c=nom> <Def1.n=V1.n> =text> #NPTN1 
NPDefN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTN1 
NPDefN = "���" Pa1<��������> NPTN1  <Pa1=NPTN1 > =text> #NPTN1 
NPDefN = {"�.�." | "�" "." "�" "."}<1,1> NPTN1  =text> #NPTN1 
NPDefN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTN1 <c=ins> <Pa1=Def1>  =text> #NPTN1 
NPDefN = Def1 "," Pa1<��������> NPTN1 <c=nom> <Pa1=Def1> =text> #NPTN1 
NPDefN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTN1 <c=ins> <Def1=Pa1> =text> #NPTN1 
NPDefN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTN1 <c=ins> <Pn1=V1> =text> #NPTN1 
NPDefN = "���" NPTN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "���" NPTN1 <c=ins> "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "���" NPTN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "�����" "��������" "���" NPTN1 <c=ins> Def1<c=acc> =text> #NPTN1 
NPDefN = "������" "�������" NPTN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "������" "�������" NPTN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "���" "��������" NPTN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "���" NPTN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "���" NPTN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTN1 
NPDefN = "���" NPTN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTN1 
NPDefN = "���" "��������" NPTN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTN1 .n=V1.n> =text> #NPTN1 
NPDefN = N1<������> NPTN1 <c=nom> =text> #NPTN1 
NPDefN = "���" "������" NPTN1 <c=nom> "����������" Def1<c=nom> =text> #NPTN1 
NPDefN = NPTN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTN1 
NPDefN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTN1 <c=gen> =text> #NPTN1 
NPDefN = NPTN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTN1 
NPDefN = NPTN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTN1 .c=Def1.c> =text> #NPTN1 
NPDefN = Pr1 NPTN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTN1 .c=Def1.c> =text> #NPTN1 
NPDefN = AP1 "\(" {"�.�."|"��" "����"|"�." "�."}<1,1> {W1} "\)" NPNMSPTWON1 <AP1=NPNMSPTWON1> =text> #NPNMSPTWON1

NPSDefNA = "��������" Pn1<�����> N1<c=acc> [PG1] NPTAN1<c=ins> <N1=PG1,Pn1=NPTAN1,NPTAN1.n=N1.n, NPTAN1.g=N1.g, N1.a=NPTAN1.a> =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1<c=nom> [PG1] ["������"|"�"] V1<����������, t=pres, p=3, m=ind> NPTAN1<c=ins> <N1=PG1,N1.g=NPTAN1.g, N1.a=NPTAN1.a, N1.n=V1.n=NPTAN1.n> =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1<c=acc> [PG1] ["�����"] "��������" NPTAN1<c=ins> <N1=PG1,N1.g=NPTAN1.g, N1.a=NPTAN1.a, N1.n=NPTAN1.n>  =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = "�������" N1<c=acc> [PG1] [Prep1<c=prep>] NPTAN1<c=ins> <N1=PG1,NPTAN1.n=N1.n, NPTAN1.g=N1.g, N1.a=NPTAN1.a> =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1<c=acc> [PG1] "�������" NPTAN1<c=ins> <N1=PG1,NPTAN1.n=N1.n, NPTAN1.g=N1.g, N1.a=NPTAN1.a> =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1<c=nom> [PG1] ["�������"] "�������" NPTAN1<c=ins> <N1=PG1,NPTAN1.n=N1.n, NPTAN1.g=N1.g, N1.a=NPTAN1.a>  =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1<c=acc> [PG1] ["�����"] "�����" "��������" NPTAN1<c=ins> <N1=PG1,NPTAN1.n=N1.n, NPTAN1.g=N1.g, N1.a=NPTAN1.a>  =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1<c=acc> [PG1] "��" "��������" NPTAN1<c=ins> <N1=PG1,NPTAN1.n=N1.n, NPTAN1.g=N1.g, N1.a=NPTAN1.a>  =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"
NPSDefN = N1 [PG1]"," ["�������"|"������"] Pa1<�������> NPTAN1<c=ins> <N1=PG1,N1.g=NPTAN1.g, N1.a=NPTAN1.a, N1.n=V1.n=NPTAN1.n> =text> "AP1[" #NPTAN1 "] N1[" #N1 "]  [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 [AP1~]N1]"

NMSPNN = N1 N2<c=gen> (N1) =text> N1 N2<c=gen>
MSPNN = N1 N2<c=gen> (N1) =text> N1 N2<c=gen>
NMSPTWONN = N1 N2<c=gen> (N1) =text> N1 N2<c=gen>
TSNN = MSPNN1 ["\("MSPNN2"\)"] <MSPNN1.c=MSPNN2.c> (MSPNN1) =text> MSPNN1
TNN = TSNN1 [[","] "���" ["������"] TSNN2] <TSNN1.c=TSNN2.c> (TSNN1) =text> TSNN1
TANNSyn = AP1 ["\("AP2"\)"]  <AP1=AP2> (AP1) =text> AP1
TANN = TANNSyn1 [[","] "���" TANNSyn2 [","]] <TANNSyn1=TANNSyn2> (TANNSyn1) =text> TANNSyn1
TNNN = NMSPNN1 [PG1] <NMSPNN1=PG1> (NMSPNN1) =text> NMSPNN1

DefNN = TNN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNN1
DefNN = '���' TNN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNN1
DefNN = TNN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNN1
DefNN = TNN1<c=ins> '��' '��������' Def1<c=acc> =text> #TNN1
DefNN = '���' TNN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TNN1
DefNN = TNN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNN1
DefNN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TNN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNN1
DefNN = TNN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TNN1
DefNN = Def1<c=acc> ["��"] "�����" "��������" TNN1<c=ins>  =text> #TNN1
DefNN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TNN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNN1
DefNN = Def1<c=acc> "�����" "��������" TNN1<c=ins>  =text> #TNN1
DefNN = Def1<c=acc> ["������"] "�������" TNN1<c=ins> =text> #TNN1
DefNN = "�������" TNN1<c=ins> Def1<c=acc>  =text> #TNN1
DefNN = "�������" Def1<c=acc> TNN1<c=ins> =text> #TNN1
DefNN = "�������" Def1<c=acc> TNN1<c=nom> =text> #TNN1
DefNN = Def1<c=acc> "," Pn1<�������> "�������" TNN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNN1
DefNN = Def1<c=acc> "�����" "�������" TNN1<c=ins> =text> #TNN1
DefNN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TNN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNN1
DefNN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TNN1<c=ins> =text> #TNN1
DefNN = "�����" "����" Pa1<�������, f=short> TNN1<c=ins> <Pa1.n=TNN1.n> =text> #TNN1
DefNN = TNN1<c=ins> ["\("MSPNN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TNN1
DefNN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TNN1<c=ins> <Def1.n=V1.n> =text> #TNN1
DefNN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TNN1<c=ins> <V1.n=Def1.n> =text> TNN1
DefNN = Def1<c=nom> "����������" ["�����" "���������" "-"] TNN1<c=nom> =text> #TNN1
DefNN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TNN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNN1
DefNN = TNN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNN1
DefNN = Def1<c=acc> "�������" "��������" TNN1<c=ins> =text> #TNN1
DefNN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TNN1<c=ins> =text> #TNN1
DefNN = TNN1<c=ins> "��������" Def1<c=acc> =text> #TNN1
DefNN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TNN1<c=ins> <Def1=Pa1> =text> #TNN1
DefNN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TNN1<c=nom> <Pa1.n=N1.n> =text> #TNN1
DefNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNN1
DefNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNN1<c=nom> <Def1.n=V1.n> =text> #TNN1
DefNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNN1<c=gen> <Def1.n=V1.n> =text> #TNN1
DefNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNN1<c=nom> <Def1.n=V1.n> =text> #TNN1
DefNN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TNN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNN1
DefNN = "���" Pa1<��������> TNN1 <Pa1=TNN1> =text> #TNN1
DefNN = {"�.�." | "�" "." "�" "."}<1,1> TNN1 =text> #TNN1
DefNN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TNN1<c=ins> <Pa1=Def1>  =text> #TNN1
DefNN = Def1 "," Pa1<��������> TNN1<c=nom> <Pa1=Def1> =text> #TNN1
DefNN = Def1 { "," | "\(" }<1,1> Pa1<�������> TNN1<c=ins> <Def1=Pa1> =text> #TNN1
DefNN = Pn1 V1<��������, t=pres, p=3, m=ind> TNN1<c=ins> <Pn1=V1> =text> #TNN1
DefNN = "���" TNN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNN1
DefNN = "���" TNN1<c=ins> "��������" Def1<c=acc> =text> #TNN1
DefNN = "���" TNN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TNN1
DefNN = "�����" "��������" "���" TNN1<c=ins> Def1<c=acc> =text> #TNN1
DefNN = "������" "�������" TNN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNN1
DefNN = "������" "�������" TNN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNN1
DefNN = "���" "��������" TNN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TNN1
DefNN = "���" TNN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNN1
DefNN = "���" TNN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNN1
DefNN = "���" TNN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNN1
DefNN = "���" "��������" TNN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TNN1.n=V1.n> =text> #TNN1
DefNN = N1<������> TNN1<c=nom> =text> #TNN1
DefNN = "���" "������" TNN1<c=nom> "����������" Def1<c=nom> =text> #TNN1
DefNN = TNN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TNN1
DefNN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TNN1<c=gen> =text> #TNN1
DefNN = TNN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TNN1
DefNN = TNN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TNN1.c=Def1.c> =text> #TNN1
DefNN = Pr1 TNN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TNN1.c=Def1.c> =text> #TNN1
DefNN = AP1 "\(" {"�.�."|"��" "����"|"�." "�."}<1,1> {W1} "\)" NMSPTWONN1 <AP1=NMSPTWONN1> =text> #NMSPTWONN1

SDefNNA = "��������" Pn1<�����> TNNN1<c=acc> TANN1<c=ins> <Pn1=TANN1,TANN1.n=TNNN1.n, TANN1.g=TNNN1.g, TNNN1.a=TANN1.a> =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1<c=nom> ["������"|"�"] V1<����������, t=pres, p=3, m=ind> TANN1<c=ins> <TNNN1.g=TANN1.g, TNNN1.a=TANN1.a, TNNN1.n=V1.n=TANN1.n> =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1<c=acc> ["�����"] "��������" TANN1<c=ins> <TNNN1.g=TANN1.g, TNNN1.a=TANN1.a, TNNN1.n=TANN1.n>  =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = "�������" TNNN1<c=acc> [Prep1<c=prep>] TANN1<c=ins> <TANN1.n=TNNN1.n, TANN1.g=TNNN1.g, TNNN1.a=TANN1.a> =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1<c=acc> "�������" TANN1<c=ins> <TANN1.n=TNNN1.n, TANN1.g=TNNN1.g, TNNN1.a=TANN1.a>  =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1<c=nom> ["�������"] "�������" TANN1<c=ins> <TANN1.n=TNNN1.n, TANN1.g=TNNN1.g, TNNN1.a=TANN1.a>  =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1<c=acc> ["�����"] "�����" "��������" TANN1<c=ins> <TANN1.n=TNNN1.n, TANN1.g=TNNN1.g, TNNN1.a=TANN1.a>  =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1<c=acc> "��" "��������" TANN1<c=ins> <TANN1.n=TNNN1.n, TANN1.g=TNNN1.g, TNNN1.a=TANN1.a>  =text> TANN1 #TNNN1 <TANN1~>TNNN1>
SDefNN = TNNN1"," ["�������"|"������"] Pa1<�������> TANN1<c=ins> <TNNN1.g=TANN1.g, TNNN1.a=TANN1.a, TNNN1.n=V1.n=TANN1.n> =text> TANN1 #TNNN1 <TANN1~>TNNN1>

NPMSPNN = N1 N2<c=gen> (N1) =text> "N1[" #N1 "] N2[" #N2 "] =text] #N1 N2[c=gen]" 
NPNMSPTWONN = N1 N2<c=gen> (N1) =text> "N1[" #N1 "] N2[" #N2 "] =text] #N1 N2[c=gen]" 
NPTANNSyn = AP1 ["\("AP2"\)"]  <AP1=AP2> (AP1) =text> AP1
NPTNNSyn = NPMSPNN1 ["\("NPMSPNN2"\)"] <NPMSPNN1.c=NPMSPNN2.c> (NPMSPNN1) =text> NPMSPNN1
NPTNN = NPTNNSyn1 [[","] "���" ["������"] NPTNNSyn2] <NPTNNSyn1.c=NPTNNSyn2.c> (NPTNNSyn1) =text> NPTNNSyn1
NPTANN = NPTANNSyn1 [[","] "���" NPTANNSyn2 [","]] <NPTANNSyn1=NPTANNSyn2> (NPTANNSyn1) =text> NPTANNSyn1

NPDefNN = NPTNN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNN1 
NPDefNN = '���' NPTNN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTNN1 
NPDefNN = '���' NPTNN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTNN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNN1 
NPDefNN = NPTNN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTNN1 
NPDefNN = Def1<c=acc> ["��"] "�����" "��������" NPTNN1 <c=ins>  =text> #NPTNN1 
NPDefNN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTNN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNN1 
NPDefNN = Def1<c=acc> "�����" "��������" NPTNN1 <c=ins>  =text> #NPTNN1 
NPDefNN = Def1<c=acc> ["������"] "�������" NPTNN1 <c=ins> =text> #NPTNN1 
NPDefNN = "�������" NPTNN1 <c=ins> Def1<c=acc>  =text> #NPTNN1 
NPDefNN = "�������" Def1<c=acc> NPTNN1 <c=ins> =text> #NPTNN1 
NPDefNN = "�������" Def1<c=acc> NPTNN1 <c=nom> =text> #NPTNN1 
NPDefNN = Def1<c=acc> "," Pn1<�������> "�������" NPTNN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNN1 
NPDefNN = Def1<c=acc> "�����" "�������" NPTNN1 <c=ins> =text> #NPTNN1 
NPDefNN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTNN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNN1 
NPDefNN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTNN1 <c=ins> =text> #NPTNN1 
NPDefNN = "�����" "����" Pa1<�������, f=short> NPTNN1 <c=ins> <Pa1.n=NPTNN1 .n> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTNN1 
NPDefNN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTNN1 <c=ins> <Def1.n=V1.n> =text> #NPTNN1 
NPDefNN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTNN1 <c=ins> <V1.n=Def1.n> =text> #NPTNN1 
NPDefNN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTNN1 <c=nom> =text> #NPTNN1 
NPDefNN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTNN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = Def1<c=acc> "�������" "��������" NPTNN1 <c=ins> =text> #NPTNN1 
NPDefNN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTNN1 <c=ins> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=ins> "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTNN1 <c=ins> <Def1=Pa1> =text> #NPTNN1 
NPDefNN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTNN1 <c=nom> <Pa1.n=N1.n> =text> #NPTNN1 
NPDefNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNN1 
NPDefNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNN1 <c=nom> <Def1.n=V1.n> =text> #NPTNN1 
NPDefNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNN1 <c=gen> <Def1.n=V1.n> =text> #NPTNN1 
NPDefNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNN1 <c=nom> <Def1.n=V1.n> =text> #NPTNN1 
NPDefNN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTNN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNN1 
NPDefNN = "���" Pa1<��������> NPTNN1  <Pa1=NPTNN1 > =text> #NPTNN1 
NPDefNN = {"�.�." | "�" "." "�" "."}<1,1> NPTNN1  =text> #NPTNN1 
NPDefNN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTNN1 <c=ins> <Pa1=Def1>  =text> #NPTNN1 
NPDefNN = Def1 "," Pa1<��������> NPTNN1 <c=nom> <Pa1=Def1> =text> #NPTNN1 
NPDefNN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTNN1 <c=ins> <Def1=Pa1> =text> #NPTNN1 
NPDefNN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTNN1 <c=ins> <Pn1=V1> =text> #NPTNN1 
NPDefNN = "���" NPTNN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "���" NPTNN1 <c=ins> "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "���" NPTNN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "�����" "��������" "���" NPTNN1 <c=ins> Def1<c=acc> =text> #NPTNN1 
NPDefNN = "������" "�������" NPTNN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "������" "�������" NPTNN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "���" "��������" NPTNN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "���" NPTNN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "���" NPTNN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNN1 
NPDefNN = "���" NPTNN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNN1 
NPDefNN = "���" "��������" NPTNN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTNN1 .n=V1.n> =text> #NPTNN1 
NPDefNN = N1<������> NPTNN1 <c=nom> =text> #NPTNN1 
NPDefNN = "���" "������" NPTNN1 <c=nom> "����������" Def1<c=nom> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTNN1 
NPDefNN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTNN1 <c=gen> =text> #NPTNN1 
NPDefNN = NPTNN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTNN1 
NPDefNN = NPTNN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTNN1 .c=Def1.c> =text> #NPTNN1 
NPDefNN = Pr1 NPTNN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTNN1 .c=Def1.c> =text> #NPTNN1 
NPDefNN = AP1 "\(" {"�.�."|"��" "����"|"�." "�."}<1,1> {W1} "\)" NPNMSPTWONN1 <AP1=NPNMSPTWONN1> =text> #NPNMSPTWONN1

NPSDefNNA = "��������" Pn1<�����> N1<c=acc> N2<c=gen> [PG1] NPTANN1<c=ins> <N1=PG1,Pn1=NPTANN1,NPTANN1.n=N1.n, NPTANN1.g=N1.g, N1.a=NPTANN1.a> =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=nom> N2<c=gen> [PG1] ["������"|"�"] V1<����������, t=pres, p=3, m=ind> NPTANN1<c=ins> <N1=PG1,N1.g=NPTANN1.g, N1.a=NPTANN1.a, N1.n=V1.n=NPTANN1.n> =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=acc> N2<c=gen> [PG1] ["�����"] "��������" NPTANN1<c=ins> <N1=PG1,N1.g=NPTANN1.g, N1.a=NPTANN1.a, N1.n=NPTANN1.n>  =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = "�������" N1<c=acc> N2<c=gen> [PG1] [Prep1<c=prep>] NPTANN1<c=ins> <N1=PG1,NPTANN1.n=N1.n, NPTANN1.g=N1.g, N1.a=NPTANN1.a> =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=acc> N2<c=gen> [PG1] "�������" NPTANN1<c=ins> <N1=PG1,NPTANN1.n=N1.n, NPTANN1.g=N1.g, N1.a=NPTANN1.a> =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=nom> N2<c=gen> [PG1] ["�������"] "�������" NPTANN1<c=ins> <N1=PG1,NPTANN1.n=N1.n, NPTANN1.g=N1.g, N1.a=NPTANN1.a>  =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=acc> N2<c=gen> [PG1] ["�����"] "�����" "��������" NPTANN1<c=ins> <N1=PG1,NPTANN1.n=N1.n, NPTANN1.g=N1.g, N1.a=NPTANN1.a>  =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=acc> N2<c=gen> [PG1] "��" "��������" NPTANN1<c=ins> <N1=PG1,NPTANN1.n=N1.n, NPTANN1.g=N1.g, N1.a=NPTANN1.a>  =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"
NPSDefNN = N1<c=acc> N2<c=gen> [PG1]"," ["�������"|"������"] Pa1<�������> NPTANN1<c=ins> <N1=PG1,N1.g=NPTANN1.g, N1.a=NPTANN1.a, N1.n=V1.n=NPTANN1.n> =text> "AP1[" #NPTANN1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [AP1.n=N1.n, AP1.g=N1.g, N1.a=AP1.a] =text] AP1 #N1 N2[c=gen] [AP1~]N1]"

MSPAN = A1 N1 <A1=N1> (N1) =text> A1 N1 <A1~>N1>
TSAN = MSPAN1 ["\("MSPAN2"\)"] <MSPAN1.c=MSPAN2.c> (MSPAN1) =text> MSPAN1
TANTWO = TSAN1 [[","] "���" ["������"] TSAN2] <TSAN1.c=TSAN2.c> (TSAN1) =text> TSAN1

DefAN = TANTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TANTWO1
DefAN = '���' TANTWO1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TANTWO1
DefAN = TANTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TANTWO1
DefAN = TANTWO1<c=ins> '��' '��������' Def1<c=acc> =text> #TANTWO1
DefAN = '���' TANTWO1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TANTWO1
DefAN = TANTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TANTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TANTWO1
DefAN = TANTWO1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TANTWO1
DefAN = Def1<c=acc> ["��"] "�����" "��������" TANTWO1<c=ins>  =text> #TANTWO1
DefAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TANTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TANTWO1
DefAN = Def1<c=acc> "�����" "��������" TANTWO1<c=ins>  =text> #TANTWO1
DefAN = Def1<c=acc> ["������"] "�������" TANTWO1<c=ins> =text> #TANTWO1
DefAN = "�������" TANTWO1<c=ins> Def1<c=acc>  =text> #TANTWO1
DefAN = "�������" Def1<c=acc> TANTWO1<c=ins> =text> #TANTWO1
DefAN = "�������" Def1<c=acc> TANTWO1<c=nom> =text> #TANTWO1
DefAN = Def1<c=acc> "," Pn1<�������> "�������" TANTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TANTWO1
DefAN = Def1<c=acc> "�����" "�������" TANTWO1<c=ins> =text> #TANTWO1
DefAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TANTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TANTWO1
DefAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TANTWO1<c=ins> =text> #TANTWO1
DefAN = "�����" "����" Pa1<�������, f=short> TANTWO1<c=ins> <Pa1.n=TANTWO1.n> =text> #TANTWO1
DefAN = TANTWO1<c=ins> ["\("MSPAN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TANTWO1
DefAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TANTWO1<c=ins> <Def1.n=V1.n> =text> #TANTWO1
DefAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TANTWO1<c=ins> <V1.n=Def1.n> =text> TANTWO1
DefAN = Def1<c=nom> "����������" ["�����" "���������" "-"] TANTWO1<c=nom> =text> #TANTWO1
DefAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TANTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TANTWO1
DefAN = TANTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = Def1<c=acc> "�������" "��������" TANTWO1<c=ins> =text> #TANTWO1
DefAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TANTWO1<c=ins> =text> #TANTWO1
DefAN = TANTWO1<c=ins> "��������" Def1<c=acc> =text> #TANTWO1
DefAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TANTWO1<c=ins> <Def1=Pa1> =text> #TANTWO1
DefAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TANTWO1<c=nom> <Pa1.n=N1.n> =text> #TANTWO1
DefAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TANTWO1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TANTWO1
DefAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TANTWO1<c=nom> <Def1.n=V1.n> =text> #TANTWO1
DefAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TANTWO1<c=gen> <Def1.n=V1.n> =text> #TANTWO1
DefAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TANTWO1<c=nom> <Def1.n=V1.n> =text> #TANTWO1
DefAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TANTWO1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TANTWO1
DefAN = "���" Pa1<��������> TANTWO1 <Pa1=TANTWO1> =text> #TANTWO1
DefAN = {"�.�." | "�" "." "�" "."}<1,1> TANTWO1 =text> #TANTWO1
DefAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TANTWO1<c=ins> <Pa1=Def1>  =text> #TANTWO1
DefAN = Def1 "," Pa1<��������> TANTWO1<c=nom> <Pa1=Def1> =text> #TANTWO1
DefAN = Def1 { "," | "\(" }<1,1> Pa1<�������> TANTWO1<c=ins> <Def1=Pa1> =text> #TANTWO1
DefAN = Pn1 V1<��������, t=pres, p=3, m=ind> TANTWO1<c=ins> <Pn1=V1> =text> #TANTWO1
DefAN = "���" TANTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "���" TANTWO1<c=ins> "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "���" TANTWO1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "�����" "��������" "���" TANTWO1<c=ins> Def1<c=acc> =text> #TANTWO1
DefAN = "������" "�������" TANTWO1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "������" "�������" TANTWO1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "���" "��������" TANTWO1<c=nom> "�����" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "���" TANTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "���" TANTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TANTWO1
DefAN = "���" TANTWO1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TANTWO1
DefAN = "���" "��������" TANTWO1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TANTWO1.n=V1.n> =text> #TANTWO1
DefAN = N1<������> TANTWO1<c=nom> =text> #TANTWO1
DefAN = "���" "������" TANTWO1<c=nom> "����������" Def1<c=nom> =text> #TANTWO1
DefAN = TANTWO1<c=nom> ["�"] "����" Def1<c=nom> =text> #TANTWO1
DefAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TANTWO1<c=gen> =text> #TANTWO1
DefAN = TANTWO1<c=nom> ["�"] "���" Def1<c=nom> =text> #TANTWO1
DefAN = TANTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TANTWO1.c=Def1.c> =text> #TANTWO1
DefAN = Pr1 TANTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TANTWO1.c=Def1.c> =text> #TANTWO1

NPMSPAN = A1 N1 <A1=N1> (N1) =text> "A1[" #A1 "] N1[" #N1 "] [A1=N1] (N1) =text] A1 #N1 [A1~]N1]"
NPTANTWOSyn = NPMSPAN1 ["\("NPMSPAN2"\)"] <NPMSPAN1.c=NPMSPAN2.c> (NPMSPAN1) =text> NPMSPAN1
NPTANTWO = NPTANTWOSyn1 [[","] "���" ["������"] NPTANTWOSyn2] <NPTANTWOSyn1.c=NPTANTWOSyn2.c> (NPTANTWOSyn1) =text> NPTANTWOSyn1

NPDefAN = NPTANTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTANTWO1 
NPDefAN = '���' NPTANTWO1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTANTWO1 
NPDefAN = '���' NPTANTWO1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTANTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> ["��"] "�����" "��������" NPTANTWO1 <c=ins>  =text> #NPTANTWO1 
NPDefAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTANTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> "�����" "��������" NPTANTWO1 <c=ins>  =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> ["������"] "�������" NPTANTWO1 <c=ins> =text> #NPTANTWO1 
NPDefAN = "�������" NPTANTWO1 <c=ins> Def1<c=acc>  =text> #NPTANTWO1 
NPDefAN = "�������" Def1<c=acc> NPTANTWO1 <c=ins> =text> #NPTANTWO1 
NPDefAN = "�������" Def1<c=acc> NPTANTWO1 <c=nom> =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> "," Pn1<�������> "�������" NPTANTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> "�����" "�������" NPTANTWO1 <c=ins> =text> #NPTANTWO1 
NPDefAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTANTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTANTWO1 <c=ins> =text> #NPTANTWO1 
NPDefAN = "�����" "����" Pa1<�������, f=short> NPTANTWO1 <c=ins> <Pa1.n=NPTANTWO1 .n> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTANTWO1 <c=ins> <Def1.n=V1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTANTWO1 <c=ins> <V1.n=Def1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTANTWO1 <c=nom> =text> #NPTANTWO1 
NPDefAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTANTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> "�������" "��������" NPTANTWO1 <c=ins> =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTANTWO1 <c=ins> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTANTWO1 <c=ins> <Def1=Pa1> =text> #NPTANTWO1 
NPDefAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTANTWO1 <c=nom> <Pa1.n=N1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTANTWO1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTANTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTANTWO1 <c=gen> <Def1.n=V1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTANTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTANTWO1 
NPDefAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTANTWO1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTANTWO1 
NPDefAN = "���" Pa1<��������> NPTANTWO1  <Pa1=NPTANTWO1 > =text> #NPTANTWO1 
NPDefAN = {"�.�." | "�" "." "�" "."}<1,1> NPTANTWO1  =text> #NPTANTWO1 
NPDefAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTANTWO1 <c=ins> <Pa1=Def1>  =text> #NPTANTWO1 
NPDefAN = Def1 "," Pa1<��������> NPTANTWO1 <c=nom> <Pa1=Def1> =text> #NPTANTWO1 
NPDefAN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTANTWO1 <c=ins> <Def1=Pa1> =text> #NPTANTWO1 
NPDefAN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTANTWO1 <c=ins> <Pn1=V1> =text> #NPTANTWO1 
NPDefAN = "���" NPTANTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "���" NPTANTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "���" NPTANTWO1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "�����" "��������" "���" NPTANTWO1 <c=ins> Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "������" "�������" NPTANTWO1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "������" "�������" NPTANTWO1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "���" "��������" NPTANTWO1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "���" NPTANTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "���" NPTANTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTANTWO1 
NPDefAN = "���" NPTANTWO1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTANTWO1 
NPDefAN = "���" "��������" NPTANTWO1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTANTWO1 .n=V1.n> =text> #NPTANTWO1 
NPDefAN = N1<������> NPTANTWO1 <c=nom> =text> #NPTANTWO1 
NPDefAN = "���" "������" NPTANTWO1 <c=nom> "����������" Def1<c=nom> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTANTWO1 
NPDefAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTANTWO1 <c=gen> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTANTWO1 
NPDefAN = NPTANTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTANTWO1 .c=Def1.c> =text> #NPTANTWO1 
NPDefAN = Pr1 NPTANTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTANTWO1 .c=Def1.c> =text> #NPTANTWO1

MSPPN = Pa1 N1 <Pa1=N1> (N1) =text> Pa1 N1 <Pa1~>N1>
TSPN = MSPPN1 ["\("MSPPN2"\)"] <MSPPN1.c=MSPPN2.c> (MSPPN1) =text> MSPPN1
TPNTWO = TSPN1 [[","] "���" ["������"] TSPN2] <TSPN1.c=TSPN2.c> (TSPN1) =text> TSPN1

DefPN = TPNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPNTWO1
DefPN = '���' TPNTWO1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPNTWO1
DefPN = TPNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPNTWO1
DefPN = TPNTWO1<c=ins> '��' '��������' Def1<c=acc> =text> #TPNTWO1
DefPN = '���' TPNTWO1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TPNTWO1
DefPN = TPNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TPNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPNTWO1
DefPN = TPNTWO1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TPNTWO1
DefPN = Def1<c=acc> ["��"] "�����" "��������" TPNTWO1<c=ins>  =text> #TPNTWO1
DefPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TPNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPNTWO1
DefPN = Def1<c=acc> "�����" "��������" TPNTWO1<c=ins>  =text> #TPNTWO1
DefPN = Def1<c=acc> ["������"] "�������" TPNTWO1<c=ins> =text> #TPNTWO1
DefPN = "�������" TPNTWO1<c=ins> Def1<c=acc>  =text> #TPNTWO1
DefPN = "�������" Def1<c=acc> TPNTWO1<c=ins> =text> #TPNTWO1
DefPN = "�������" Def1<c=acc> TPNTWO1<c=nom> =text> #TPNTWO1
DefPN = Def1<c=acc> "," Pn1<�������> "�������" TPNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPNTWO1
DefPN = Def1<c=acc> "�����" "�������" TPNTWO1<c=ins> =text> #TPNTWO1
DefPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TPNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPNTWO1
DefPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TPNTWO1<c=ins> =text> #TPNTWO1
DefPN = "�����" "����" Pa1<�������, f=short> TPNTWO1<c=ins> <Pa1.n=TPNTWO1.n> =text> #TPNTWO1
DefPN = TPNTWO1<c=ins> ["\("MSPPN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TPNTWO1
DefPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TPNTWO1<c=ins> <Def1.n=V1.n> =text> #TPNTWO1
DefPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TPNTWO1<c=ins> <V1.n=Def1.n> =text> TPNTWO1
DefPN = Def1<c=nom> "����������" ["�����" "���������" "-"] TPNTWO1<c=nom> =text> #TPNTWO1
DefPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TPNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPNTWO1
DefPN = TPNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = Def1<c=acc> "�������" "��������" TPNTWO1<c=ins> =text> #TPNTWO1
DefPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TPNTWO1<c=ins> =text> #TPNTWO1
DefPN = TPNTWO1<c=ins> "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TPNTWO1<c=ins> <Def1=Pa1> =text> #TPNTWO1
DefPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TPNTWO1<c=nom> <Pa1.n=N1.n> =text> #TPNTWO1
DefPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPNTWO1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPNTWO1
DefPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPNTWO1<c=nom> <Def1.n=V1.n> =text> #TPNTWO1
DefPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPNTWO1<c=gen> <Def1.n=V1.n> =text> #TPNTWO1
DefPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPNTWO1<c=nom> <Def1.n=V1.n> =text> #TPNTWO1
DefPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TPNTWO1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPNTWO1
DefPN = "���" Pa1<��������> TPNTWO1 <Pa1=TPNTWO1> =text> #TPNTWO1
DefPN = {"�.�." | "�" "." "�" "."}<1,1> TPNTWO1 =text> #TPNTWO1
DefPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TPNTWO1<c=ins> <Pa1=Def1>  =text> #TPNTWO1
DefPN = Def1 "," Pa1<��������> TPNTWO1<c=nom> <Pa1=Def1> =text> #TPNTWO1
DefPN = Def1 { "," | "\(" }<1,1> Pa1<�������> TPNTWO1<c=ins> <Def1=Pa1> =text> #TPNTWO1
DefPN = Pn1 V1<��������, t=pres, p=3, m=ind> TPNTWO1<c=ins> <Pn1=V1> =text> #TPNTWO1
DefPN = "���" TPNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "���" TPNTWO1<c=ins> "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "���" TPNTWO1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "�����" "��������" "���" TPNTWO1<c=ins> Def1<c=acc> =text> #TPNTWO1
DefPN = "������" "�������" TPNTWO1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "������" "�������" TPNTWO1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "���" "��������" TPNTWO1<c=nom> "�����" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "���" TPNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "���" TPNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TPNTWO1
DefPN = "���" TPNTWO1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPNTWO1
DefPN = "���" "��������" TPNTWO1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TPNTWO1.n=V1.n> =text> #TPNTWO1
DefPN = N1<������> TPNTWO1<c=nom> =text> #TPNTWO1
DefPN = "���" "������" TPNTWO1<c=nom> "����������" Def1<c=nom> =text> #TPNTWO1
DefPN = TPNTWO1<c=nom> ["�"] "����" Def1<c=nom> =text> #TPNTWO1
DefPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TPNTWO1<c=gen> =text> #TPNTWO1
DefPN = TPNTWO1<c=nom> ["�"] "���" Def1<c=nom> =text> #TPNTWO1
DefPN = TPNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TPNTWO1.c=Def1.c> =text> #TPNTWO1
DefPN = Pr1 TPNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TPNTWO1.c=Def1.c> =text> #TPNTWO1

NPMSPPN = Pa1 N1 <Pa1=N1> (N1) =text> "Pa1[" #Pa1 "] N1[" #N1 "] [Pa1=N1] (N1) =text] Pa1 #N1 [Pa1~]N1]"
NPTPNTWOSyn = NPMSPPN1 ["\("NPMSPPN2"\)"] <NPMSPPN1.c=NPMSPPN2.c> (NPMSPPN1) =text> NPMSPPN1
NPTPNTWO = NPTPNTWOSyn1 [[","] "���" ["������"] NPTPNTWOSyn2] <NPTPNTWOSyn1.c=NPTPNTWOSyn2.c> (NPTPNTWOSyn1) =text> NPTPNTWOSyn1

NPDefPN = NPTPNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPNTWO1 
NPDefPN = '���' NPTPNTWO1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTPNTWO1 
NPDefPN = '���' NPTPNTWO1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTPNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> ["��"] "�����" "��������" NPTPNTWO1 <c=ins>  =text> #NPTPNTWO1 
NPDefPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTPNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> "�����" "��������" NPTPNTWO1 <c=ins>  =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> ["������"] "�������" NPTPNTWO1 <c=ins> =text> #NPTPNTWO1 
NPDefPN = "�������" NPTPNTWO1 <c=ins> Def1<c=acc>  =text> #NPTPNTWO1 
NPDefPN = "�������" Def1<c=acc> NPTPNTWO1 <c=ins> =text> #NPTPNTWO1 
NPDefPN = "�������" Def1<c=acc> NPTPNTWO1 <c=nom> =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> "," Pn1<�������> "�������" NPTPNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> "�����" "�������" NPTPNTWO1 <c=ins> =text> #NPTPNTWO1 
NPDefPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTPNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTPNTWO1 <c=ins> =text> #NPTPNTWO1 
NPDefPN = "�����" "����" Pa1<�������, f=short> NPTPNTWO1 <c=ins> <Pa1.n=NPTPNTWO1 .n> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTPNTWO1 <c=ins> <Def1.n=V1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTPNTWO1 <c=ins> <V1.n=Def1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTPNTWO1 <c=nom> =text> #NPTPNTWO1 
NPDefPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTPNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> "�������" "��������" NPTPNTWO1 <c=ins> =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTPNTWO1 <c=ins> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTPNTWO1 <c=ins> <Def1=Pa1> =text> #NPTPNTWO1 
NPDefPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTPNTWO1 <c=nom> <Pa1.n=N1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPNTWO1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPNTWO1 <c=gen> <Def1.n=V1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTPNTWO1 
NPDefPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTPNTWO1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPNTWO1 
NPDefPN = "���" Pa1<��������> NPTPNTWO1  <Pa1=NPTPNTWO1 > =text> #NPTPNTWO1 
NPDefPN = {"�.�." | "�" "." "�" "."}<1,1> NPTPNTWO1  =text> #NPTPNTWO1 
NPDefPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTPNTWO1 <c=ins> <Pa1=Def1>  =text> #NPTPNTWO1 
NPDefPN = Def1 "," Pa1<��������> NPTPNTWO1 <c=nom> <Pa1=Def1> =text> #NPTPNTWO1 
NPDefPN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTPNTWO1 <c=ins> <Def1=Pa1> =text> #NPTPNTWO1 
NPDefPN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTPNTWO1 <c=ins> <Pn1=V1> =text> #NPTPNTWO1 
NPDefPN = "���" NPTPNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "���" NPTPNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "���" NPTPNTWO1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "�����" "��������" "���" NPTPNTWO1 <c=ins> Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "������" "�������" NPTPNTWO1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "������" "�������" NPTPNTWO1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "���" "��������" NPTPNTWO1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "���" NPTPNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "���" NPTPNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPNTWO1 
NPDefPN = "���" NPTPNTWO1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPNTWO1 
NPDefPN = "���" "��������" NPTPNTWO1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTPNTWO1 .n=V1.n> =text> #NPTPNTWO1 
NPDefPN = N1<������> NPTPNTWO1 <c=nom> =text> #NPTPNTWO1 
NPDefPN = "���" "������" NPTPNTWO1 <c=nom> "����������" Def1<c=nom> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTPNTWO1 
NPDefPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTPNTWO1 <c=gen> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTPNTWO1 
NPDefPN = NPTPNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTPNTWO1 .c=Def1.c> =text> #NPTPNTWO1 
NPDefPN = Pr1 NPTPNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTPNTWO1 .c=Def1.c> =text> #NPTPNTWO1

MSPAAN = A1 A2 N1 <A1=A2=N1> (N1) =text> A1 A2 N1 <A1~>N1, A2~>N1>
TSAAN = MSPAAN1 ["\("MSPAAN2"\)"] <MSPAAN1.c=MSPAAN2.c> (MSPAAN1) =text> MSPAAN1
TAAN = TSAAN1 [[","] "���" ["������"] TSAAN2] <TSAAN1.c=TSAAN2.c> (TSAAN1) =text> TSAAN1

DefAAN = TAAN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TAAN1
DefAAN = '���' TAAN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TAAN1
DefAAN = TAAN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TAAN1
DefAAN = TAAN1<c=ins> '��' '��������' Def1<c=acc> =text> #TAAN1
DefAAN = '���' TAAN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TAAN1
DefAAN = TAAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TAAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TAAN1
DefAAN = TAAN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TAAN1
DefAAN = Def1<c=acc> ["��"] "�����" "��������" TAAN1<c=ins>  =text> #TAAN1
DefAAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TAAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TAAN1
DefAAN = Def1<c=acc> "�����" "��������" TAAN1<c=ins>  =text> #TAAN1
DefAAN = Def1<c=acc> ["������"] "�������" TAAN1<c=ins> =text> #TAAN1
DefAAN = "�������" TAAN1<c=ins> Def1<c=acc>  =text> #TAAN1
DefAAN = "�������" Def1<c=acc> TAAN1<c=ins> =text> #TAAN1
DefAAN = "�������" Def1<c=acc> TAAN1<c=nom> =text> #TAAN1
DefAAN = Def1<c=acc> "," Pn1<�������> "�������" TAAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TAAN1
DefAAN = Def1<c=acc> "�����" "�������" TAAN1<c=ins> =text> #TAAN1
DefAAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TAAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TAAN1
DefAAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TAAN1<c=ins> =text> #TAAN1
DefAAN = "�����" "����" Pa1<�������, f=short> TAAN1<c=ins> <Pa1.n=TAAN1.n> =text> #TAAN1
DefAAN = TAAN1<c=ins> ["\("MSPAAN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TAAN1
DefAAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TAAN1<c=ins> <Def1.n=V1.n> =text> #TAAN1
DefAAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TAAN1<c=ins> <V1.n=Def1.n> =text> TAAN1
DefAAN = Def1<c=nom> "����������" ["�����" "���������" "-"] TAAN1<c=nom> =text> #TAAN1
DefAAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TAAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TAAN1
DefAAN = TAAN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = Def1<c=acc> "�������" "��������" TAAN1<c=ins> =text> #TAAN1
DefAAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TAAN1<c=ins> =text> #TAAN1
DefAAN = TAAN1<c=ins> "��������" Def1<c=acc> =text> #TAAN1
DefAAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TAAN1<c=ins> <Def1=Pa1> =text> #TAAN1
DefAAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TAAN1<c=nom> <Pa1.n=N1.n> =text> #TAAN1
DefAAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TAAN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TAAN1
DefAAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TAAN1<c=nom> <Def1.n=V1.n> =text> #TAAN1
DefAAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TAAN1<c=gen> <Def1.n=V1.n> =text> #TAAN1
DefAAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TAAN1<c=nom> <Def1.n=V1.n> =text> #TAAN1
DefAAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TAAN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TAAN1
DefAAN = "���" Pa1<��������> TAAN1 <Pa1=TAAN1> =text> #TAAN1
DefAAN = {"�.�." | "�" "." "�" "."}<1,1> TAAN1 =text> #TAAN1
DefAAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TAAN1<c=ins> <Pa1=Def1>  =text> #TAAN1
DefAAN = Def1 "," Pa1<��������> TAAN1<c=nom> <Pa1=Def1> =text> #TAAN1
DefAAN = Def1 { "," | "\(" }<1,1> Pa1<�������> TAAN1<c=ins> <Def1=Pa1> =text> #TAAN1
DefAAN = Pn1 V1<��������, t=pres, p=3, m=ind> TAAN1<c=ins> <Pn1=V1> =text> #TAAN1
DefAAN = "���" TAAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "���" TAAN1<c=ins> "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "���" TAAN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "�����" "��������" "���" TAAN1<c=ins> Def1<c=acc> =text> #TAAN1
DefAAN = "������" "�������" TAAN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "������" "�������" TAAN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "���" "��������" TAAN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "���" TAAN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "���" TAAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TAAN1
DefAAN = "���" TAAN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TAAN1
DefAAN = "���" "��������" TAAN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TAAN1.n=V1.n> =text> #TAAN1
DefAAN = N1<������> TAAN1<c=nom> =text> #TAAN1
DefAAN = "���" "������" TAAN1<c=nom> "����������" Def1<c=nom> =text> #TAAN1
DefAAN = TAAN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TAAN1
DefAAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TAAN1<c=gen> =text> #TAAN1
DefAAN = TAAN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TAAN1
DefAAN = TAAN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TAAN1.c=Def1.c> =text> #TAAN1
DefAAN = Pr1 TAAN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TAAN1.c=Def1.c> =text> #TAAN1

NPMSPAAN = A1 A2 N1 <A1=A2=N1> (N1) =text>"A1[" #A1 "] A2[" #A2 "] N1[" #N1 "] [A1=A2=N1] (N1) =text] A1 A2 N1 [A1~]N1, A2~]N1]"
NPTAANSyn = NPMSPAAN1 ["\("NPMSPAAN2"\)"] <NPMSPAAN1.c=NPMSPAAN2.c> (NPMSPAAN1) =text> NPMSPAAN1
NPTAAN = NPTAANSyn1 [[","] "���" ["������"] NPTAANSyn2] <NPTAANSyn1.c=NPTAANSyn2.c> (NPTAANSyn1) =text> NPTAANSyn1

NPDefAAN = NPTAAN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTAAN1 
NPDefAAN = '���' NPTAAN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTAAN1 
NPDefAAN = '���' NPTAAN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTAAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> ["��"] "�����" "��������" NPTAAN1 <c=ins>  =text> #NPTAAN1 
NPDefAAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTAAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> "�����" "��������" NPTAAN1 <c=ins>  =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> ["������"] "�������" NPTAAN1 <c=ins> =text> #NPTAAN1 
NPDefAAN = "�������" NPTAAN1 <c=ins> Def1<c=acc>  =text> #NPTAAN1 
NPDefAAN = "�������" Def1<c=acc> NPTAAN1 <c=ins> =text> #NPTAAN1 
NPDefAAN = "�������" Def1<c=acc> NPTAAN1 <c=nom> =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> "," Pn1<�������> "�������" NPTAAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> "�����" "�������" NPTAAN1 <c=ins> =text> #NPTAAN1 
NPDefAAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTAAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTAAN1 <c=ins> =text> #NPTAAN1 
NPDefAAN = "�����" "����" Pa1<�������, f=short> NPTAAN1 <c=ins> <Pa1.n=NPTAAN1 .n> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTAAN1 <c=ins> <Def1.n=V1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTAAN1 <c=ins> <V1.n=Def1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTAAN1 <c=nom> =text> #NPTAAN1 
NPDefAAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTAAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> "�������" "��������" NPTAAN1 <c=ins> =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTAAN1 <c=ins> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=ins> "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTAAN1 <c=ins> <Def1=Pa1> =text> #NPTAAN1 
NPDefAAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTAAN1 <c=nom> <Pa1.n=N1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTAAN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTAAN1 <c=nom> <Def1.n=V1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTAAN1 <c=gen> <Def1.n=V1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTAAN1 <c=nom> <Def1.n=V1.n> =text> #NPTAAN1 
NPDefAAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTAAN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTAAN1 
NPDefAAN = "���" Pa1<��������> NPTAAN1  <Pa1=NPTAAN1 > =text> #NPTAAN1 
NPDefAAN = {"�.�." | "�" "." "�" "."}<1,1> NPTAAN1  =text> #NPTAAN1 
NPDefAAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTAAN1 <c=ins> <Pa1=Def1>  =text> #NPTAAN1 
NPDefAAN = Def1 "," Pa1<��������> NPTAAN1 <c=nom> <Pa1=Def1> =text> #NPTAAN1 
NPDefAAN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTAAN1 <c=ins> <Def1=Pa1> =text> #NPTAAN1 
NPDefAAN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTAAN1 <c=ins> <Pn1=V1> =text> #NPTAAN1 
NPDefAAN = "���" NPTAAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "���" NPTAAN1 <c=ins> "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "���" NPTAAN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "�����" "��������" "���" NPTAAN1 <c=ins> Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "������" "�������" NPTAAN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "������" "�������" NPTAAN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "���" "��������" NPTAAN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "���" NPTAAN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "���" NPTAAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTAAN1 
NPDefAAN = "���" NPTAAN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTAAN1 
NPDefAAN = "���" "��������" NPTAAN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTAAN1 .n=V1.n> =text> #NPTAAN1 
NPDefAAN = N1<������> NPTAAN1 <c=nom> =text> #NPTAAN1 
NPDefAAN = "���" "������" NPTAAN1 <c=nom> "����������" Def1<c=nom> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTAAN1 
NPDefAAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTAAN1 <c=gen> =text> #NPTAAN1 
NPDefAAN = NPTAAN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTAAN1 
NPDefAAN = NPTAAN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTAAN1 .c=Def1.c> =text> #NPTAAN1 
NPDefAAN = Pr1 NPTAAN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTAAN1 .c=Def1.c> =text> #NPTAAN1




MSPAPN = A1 Pa2 N1 <A1=Pa2=N1> (N1) =text> A1 Pa2 N1 <A1~>N1, Pa2~>N1>
TSAPN = MSPAPN1 ["\("MSPAPN2"\)"] <MSPAPN1.c=MSPAPN2.c> (MSPAPN1) =text> MSPAPN1
TAPN = TSAPN1 [[","] "���" ["������"] TSAPN2] <TSAPN1.c=TSAPN2.c> (TSAPN1) =text> TSAPN1

DefAPN = TAPN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TAPN1
DefAPN = '���' TAPN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TAPN1
DefAPN = TAPN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TAPN1
DefAPN = TAPN1<c=ins> '��' '��������' Def1<c=acc> =text> #TAPN1
DefAPN = '���' TAPN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TAPN1
DefAPN = TAPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TAPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TAPN1
DefAPN = TAPN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TAPN1
DefAPN = Def1<c=acc> ["��"] "�����" "��������" TAPN1<c=ins>  =text> #TAPN1
DefAPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TAPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TAPN1
DefAPN = Def1<c=acc> "�����" "��������" TAPN1<c=ins>  =text> #TAPN1
DefAPN = Def1<c=acc> ["������"] "�������" TAPN1<c=ins> =text> #TAPN1
DefAPN = "�������" TAPN1<c=ins> Def1<c=acc>  =text> #TAPN1
DefAPN = "�������" Def1<c=acc> TAPN1<c=ins> =text> #TAPN1
DefAPN = "�������" Def1<c=acc> TAPN1<c=nom> =text> #TAPN1
DefAPN = Def1<c=acc> "," Pn1<�������> "�������" TAPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TAPN1
DefAPN = Def1<c=acc> "�����" "�������" TAPN1<c=ins> =text> #TAPN1
DefAPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TAPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TAPN1
DefAPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TAPN1<c=ins> =text> #TAPN1
DefAPN = "�����" "����" Pa1<�������, f=short> TAPN1<c=ins> <Pa1.n=TAPN1.n> =text> #TAPN1
DefAPN = TAPN1<c=ins> ["\("MSPAPN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TAPN1
DefAPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TAPN1<c=ins> <Def1.n=V1.n> =text> #TAPN1
DefAPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TAPN1<c=ins> <V1.n=Def1.n> =text> TAPN1
DefAPN = Def1<c=nom> "����������" ["�����" "���������" "-"] TAPN1<c=nom> =text> #TAPN1
DefAPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TAPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TAPN1
DefAPN = TAPN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = Def1<c=acc> "�������" "��������" TAPN1<c=ins> =text> #TAPN1
DefAPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TAPN1<c=ins> =text> #TAPN1
DefAPN = TAPN1<c=ins> "��������" Def1<c=acc> =text> #TAPN1
DefAPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TAPN1<c=ins> <Def1=Pa1> =text> #TAPN1
DefAPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TAPN1<c=nom> <Pa1.n=N1.n> =text> #TAPN1
DefAPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TAPN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TAPN1
DefAPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TAPN1<c=nom> <Def1.n=V1.n> =text> #TAPN1
DefAPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TAPN1<c=gen> <Def1.n=V1.n> =text> #TAPN1
DefAPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TAPN1<c=nom> <Def1.n=V1.n> =text> #TAPN1
DefAPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TAPN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TAPN1
DefAPN = "���" Pa1<��������> TAPN1 <Pa1=TAPN1> =text> #TAPN1
DefAPN = {"�.�." | "�" "." "�" "."}<1,1> TAPN1 =text> #TAPN1
DefAPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TAPN1<c=ins> <Pa1=Def1>  =text> #TAPN1
DefAPN = Def1 "," Pa1<��������> TAPN1<c=nom> <Pa1=Def1> =text> #TAPN1
DefAPN = Def1 { "," | "\(" }<1,1> Pa1<�������> TAPN1<c=ins> <Def1=Pa1> =text> #TAPN1
DefAPN = Pn1 V1<��������, t=pres, p=3, m=ind> TAPN1<c=ins> <Pn1=V1> =text> #TAPN1
DefAPN = "���" TAPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "���" TAPN1<c=ins> "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "���" TAPN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "�����" "��������" "���" TAPN1<c=ins> Def1<c=acc> =text> #TAPN1
DefAPN = "������" "�������" TAPN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "������" "�������" TAPN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "���" "��������" TAPN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "���" TAPN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "���" TAPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TAPN1
DefAPN = "���" TAPN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TAPN1
DefAPN = "���" "��������" TAPN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TAPN1.n=V1.n> =text> #TAPN1
DefAPN = N1<������> TAPN1<c=nom> =text> #TAPN1
DefAPN = "���" "������" TAPN1<c=nom> "����������" Def1<c=nom> =text> #TAPN1
DefAPN = TAPN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TAPN1
DefAPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TAPN1<c=gen> =text> #TAPN1
DefAPN = TAPN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TAPN1
DefAPN = TAPN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TAPN1.c=Def1.c> =text> #TAPN1
DefAPN = Pr1 TAPN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TAPN1.c=Def1.c> =text> #TAPN1

NPMSPAPN = A1 Pa2 N1 <A1=Pa2=N1> (N1) =text>"A1[" #A1 "] Pa2[" #Pa2 "] N1[" #N1 "] [A1=Pa2=N1] (N1) =text] A1 Pa2 N1 [A1~]N1, Pa2~]N1]"
NPTAPNSyn = NPMSPAPN1 ["\("NPMSPAPN2"\)"] <NPMSPAPN1.c=NPMSPAPN2.c> (NPMSPAPN1) =text> NPMSPAPN1
NPTAPN = NPTAPNSyn1 [[","] "���" ["������"] NPTAPNSyn2] <NPTAPNSyn1.c=NPTAPNSyn2.c> (NPTAPNSyn1) =text> NPTAPNSyn1

NPDefAPN = NPTAPN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTAPN1 
NPDefAPN = '���' NPTAPN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTAPN1 
NPDefAPN = '���' NPTAPN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTAPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> ["��"] "�����" "��������" NPTAPN1 <c=ins>  =text> #NPTAPN1 
NPDefAPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTAPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> "�����" "��������" NPTAPN1 <c=ins>  =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> ["������"] "�������" NPTAPN1 <c=ins> =text> #NPTAPN1 
NPDefAPN = "�������" NPTAPN1 <c=ins> Def1<c=acc>  =text> #NPTAPN1 
NPDefAPN = "�������" Def1<c=acc> NPTAPN1 <c=ins> =text> #NPTAPN1 
NPDefAPN = "�������" Def1<c=acc> NPTAPN1 <c=nom> =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> "," Pn1<�������> "�������" NPTAPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> "�����" "�������" NPTAPN1 <c=ins> =text> #NPTAPN1 
NPDefAPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTAPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTAPN1 <c=ins> =text> #NPTAPN1 
NPDefAPN = "�����" "����" Pa1<�������, f=short> NPTAPN1 <c=ins> <Pa1.n=NPTAPN1 .n> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTAPN1 <c=ins> <Def1.n=V1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTAPN1 <c=ins> <V1.n=Def1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTAPN1 <c=nom> =text> #NPTAPN1 
NPDefAPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTAPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> "�������" "��������" NPTAPN1 <c=ins> =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTAPN1 <c=ins> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=ins> "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTAPN1 <c=ins> <Def1=Pa1> =text> #NPTAPN1 
NPDefAPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTAPN1 <c=nom> <Pa1.n=N1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTAPN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTAPN1 <c=nom> <Def1.n=V1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTAPN1 <c=gen> <Def1.n=V1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTAPN1 <c=nom> <Def1.n=V1.n> =text> #NPTAPN1 
NPDefAPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTAPN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTAPN1 
NPDefAPN = "���" Pa1<��������> NPTAPN1  <Pa1=NPTAPN1 > =text> #NPTAPN1 
NPDefAPN = {"�.�." | "�" "." "�" "."}<1,1> NPTAPN1  =text> #NPTAPN1 
NPDefAPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTAPN1 <c=ins> <Pa1=Def1>  =text> #NPTAPN1 
NPDefAPN = Def1 "," Pa1<��������> NPTAPN1 <c=nom> <Pa1=Def1> =text> #NPTAPN1 
NPDefAPN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTAPN1 <c=ins> <Def1=Pa1> =text> #NPTAPN1 
NPDefAPN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTAPN1 <c=ins> <Pn1=V1> =text> #NPTAPN1 
NPDefAPN = "���" NPTAPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "���" NPTAPN1 <c=ins> "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "���" NPTAPN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "�����" "��������" "���" NPTAPN1 <c=ins> Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "������" "�������" NPTAPN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "������" "�������" NPTAPN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "���" "��������" NPTAPN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "���" NPTAPN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "���" NPTAPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTAPN1 
NPDefAPN = "���" NPTAPN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTAPN1 
NPDefAPN = "���" "��������" NPTAPN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTAPN1 .n=V1.n> =text> #NPTAPN1 
NPDefAPN = N1<������> NPTAPN1 <c=nom> =text> #NPTAPN1 
NPDefAPN = "���" "������" NPTAPN1 <c=nom> "����������" Def1<c=nom> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTAPN1 
NPDefAPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTAPN1 <c=gen> =text> #NPTAPN1 
NPDefAPN = NPTAPN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTAPN1 
NPDefAPN = NPTAPN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTAPN1 .c=Def1.c> =text> #NPTAPN1 
NPDefAPN = Pr1 NPTAPN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTAPN1 .c=Def1.c> =text> #NPTAPN1



MSPPAN = Pa1 A2 N1 <Pa1=A2=N1> (N1) =text> Pa1 A2 N1 <Pa1~>N1, A2~>N1>
TSPAN = MSPPAN1 ["\("MSPPAN2"\)"] <MSPPAN1.c=MSPPAN2.c> (MSPPAN1) =text> MSPPAN1
TPAN = TSPAN1 [[","] "���" ["������"] TSPAN2] <TSPAN1.c=TSPAN2.c> (TSPAN1) =text> TSPAN1

DefPAN = TPAN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPAN1
DefPAN = '���' TPAN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPAN1
DefPAN = TPAN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPAN1
DefPAN = TPAN1<c=ins> '��' '��������' Def1<c=acc> =text> #TPAN1
DefPAN = '���' TPAN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TPAN1
DefPAN = TPAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TPAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPAN1
DefPAN = TPAN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TPAN1
DefPAN = Def1<c=acc> ["��"] "�����" "��������" TPAN1<c=ins>  =text> #TPAN1
DefPAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TPAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPAN1
DefPAN = Def1<c=acc> "�����" "��������" TPAN1<c=ins>  =text> #TPAN1
DefPAN = Def1<c=acc> ["������"] "�������" TPAN1<c=ins> =text> #TPAN1
DefPAN = "�������" TPAN1<c=ins> Def1<c=acc>  =text> #TPAN1
DefPAN = "�������" Def1<c=acc> TPAN1<c=ins> =text> #TPAN1
DefPAN = "�������" Def1<c=acc> TPAN1<c=nom> =text> #TPAN1
DefPAN = Def1<c=acc> "," Pn1<�������> "�������" TPAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPAN1
DefPAN = Def1<c=acc> "�����" "�������" TPAN1<c=ins> =text> #TPAN1
DefPAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TPAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPAN1
DefPAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TPAN1<c=ins> =text> #TPAN1
DefPAN = "�����" "����" Pa1<�������, f=short> TPAN1<c=ins> <Pa1.n=TPAN1.n> =text> #TPAN1
DefPAN = TPAN1<c=ins> ["\("MSPPAN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TPAN1
DefPAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TPAN1<c=ins> <Def1.n=V1.n> =text> #TPAN1
DefPAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TPAN1<c=ins> <V1.n=Def1.n> =text> TPAN1
DefPAN = Def1<c=nom> "����������" ["�����" "���������" "-"] TPAN1<c=nom> =text> #TPAN1
DefPAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TPAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPAN1
DefPAN = TPAN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = Def1<c=acc> "�������" "��������" TPAN1<c=ins> =text> #TPAN1
DefPAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TPAN1<c=ins> =text> #TPAN1
DefPAN = TPAN1<c=ins> "��������" Def1<c=acc> =text> #TPAN1
DefPAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TPAN1<c=ins> <Def1=Pa1> =text> #TPAN1
DefPAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TPAN1<c=nom> <Pa1.n=N1.n> =text> #TPAN1
DefPAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPAN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPAN1
DefPAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPAN1<c=nom> <Def1.n=V1.n> =text> #TPAN1
DefPAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPAN1<c=gen> <Def1.n=V1.n> =text> #TPAN1
DefPAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPAN1<c=nom> <Def1.n=V1.n> =text> #TPAN1
DefPAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TPAN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPAN1
DefPAN = "���" Pa1<��������> TPAN1 <Pa1=TPAN1> =text> #TPAN1
DefPAN = {"�.�." | "�" "." "�" "."}<1,1> TPAN1 =text> #TPAN1
DefPAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TPAN1<c=ins> <Pa1=Def1>  =text> #TPAN1
DefPAN = Def1 "," Pa1<��������> TPAN1<c=nom> <Pa1=Def1> =text> #TPAN1
DefPAN = Def1 { "," | "\(" }<1,1> Pa1<�������> TPAN1<c=ins> <Def1=Pa1> =text> #TPAN1
DefPAN = Pn1 V1<��������, t=pres, p=3, m=ind> TPAN1<c=ins> <Pn1=V1> =text> #TPAN1
DefPAN = "���" TPAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "���" TPAN1<c=ins> "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "���" TPAN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "�����" "��������" "���" TPAN1<c=ins> Def1<c=acc> =text> #TPAN1
DefPAN = "������" "�������" TPAN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "������" "�������" TPAN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "���" "��������" TPAN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "���" TPAN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "���" TPAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TPAN1
DefPAN = "���" TPAN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPAN1
DefPAN = "���" "��������" TPAN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TPAN1.n=V1.n> =text> #TPAN1
DefPAN = N1<������> TPAN1<c=nom> =text> #TPAN1
DefPAN = "���" "������" TPAN1<c=nom> "����������" Def1<c=nom> =text> #TPAN1
DefPAN = TPAN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TPAN1
DefPAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TPAN1<c=gen> =text> #TPAN1
DefPAN = TPAN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TPAN1
DefPAN = TPAN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TPAN1.c=Def1.c> =text> #TPAN1
DefPAN = Pr1 TPAN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TPAN1.c=Def1.c> =text> #TPAN1

NPMSPPAN = Pa1 A2 N1 <Pa1=A2=N1> (N1) =text>"Pa1[" #Pa1 "] A2[" #A2 "] N1[" #N1 "] [Pa1=A2=N1] (N1) =text] Pa1 A2 N1 [Pa1~]N1, A2~]N1]"
NPTPANSyn = NPMSPPAN1 ["\("NPMSPPAN2"\)"] <NPMSPPAN1.c=NPMSPPAN2.c> (NPMSPPAN1) =text> NPMSPPAN1
NPTPAN = NPTPANSyn1 [[","] "���" ["������"] NPTPANSyn2] <NPTPANSyn1.c=NPTPANSyn2.c> (NPTPANSyn1) =text> NPTPANSyn1

NPDefPAN = NPTPAN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPAN1 
NPDefPAN = '���' NPTPAN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTPAN1 
NPDefPAN = '���' NPTPAN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTPAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> ["��"] "�����" "��������" NPTPAN1 <c=ins>  =text> #NPTPAN1 
NPDefPAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTPAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> "�����" "��������" NPTPAN1 <c=ins>  =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> ["������"] "�������" NPTPAN1 <c=ins> =text> #NPTPAN1 
NPDefPAN = "�������" NPTPAN1 <c=ins> Def1<c=acc>  =text> #NPTPAN1 
NPDefPAN = "�������" Def1<c=acc> NPTPAN1 <c=ins> =text> #NPTPAN1 
NPDefPAN = "�������" Def1<c=acc> NPTPAN1 <c=nom> =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> "," Pn1<�������> "�������" NPTPAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> "�����" "�������" NPTPAN1 <c=ins> =text> #NPTPAN1 
NPDefPAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTPAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTPAN1 <c=ins> =text> #NPTPAN1 
NPDefPAN = "�����" "����" Pa1<�������, f=short> NPTPAN1 <c=ins> <Pa1.n=NPTPAN1 .n> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTPAN1 <c=ins> <Def1.n=V1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTPAN1 <c=ins> <V1.n=Def1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTPAN1 <c=nom> =text> #NPTPAN1 
NPDefPAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTPAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> "�������" "��������" NPTPAN1 <c=ins> =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTPAN1 <c=ins> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=ins> "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTPAN1 <c=ins> <Def1=Pa1> =text> #NPTPAN1 
NPDefPAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTPAN1 <c=nom> <Pa1.n=N1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPAN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPAN1 <c=nom> <Def1.n=V1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPAN1 <c=gen> <Def1.n=V1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPAN1 <c=nom> <Def1.n=V1.n> =text> #NPTPAN1 
NPDefPAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTPAN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPAN1 
NPDefPAN = "���" Pa1<��������> NPTPAN1  <Pa1=NPTPAN1 > =text> #NPTPAN1 
NPDefPAN = {"�.�." | "�" "." "�" "."}<1,1> NPTPAN1  =text> #NPTPAN1 
NPDefPAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTPAN1 <c=ins> <Pa1=Def1>  =text> #NPTPAN1 
NPDefPAN = Def1 "," Pa1<��������> NPTPAN1 <c=nom> <Pa1=Def1> =text> #NPTPAN1 
NPDefPAN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTPAN1 <c=ins> <Def1=Pa1> =text> #NPTPAN1 
NPDefPAN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTPAN1 <c=ins> <Pn1=V1> =text> #NPTPAN1 
NPDefPAN = "���" NPTPAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "���" NPTPAN1 <c=ins> "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "���" NPTPAN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "�����" "��������" "���" NPTPAN1 <c=ins> Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "������" "�������" NPTPAN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "������" "�������" NPTPAN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "���" "��������" NPTPAN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "���" NPTPAN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "���" NPTPAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPAN1 
NPDefPAN = "���" NPTPAN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPAN1 
NPDefPAN = "���" "��������" NPTPAN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTPAN1 .n=V1.n> =text> #NPTPAN1 
NPDefPAN = N1<������> NPTPAN1 <c=nom> =text> #NPTPAN1 
NPDefPAN = "���" "������" NPTPAN1 <c=nom> "����������" Def1<c=nom> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTPAN1 
NPDefPAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTPAN1 <c=gen> =text> #NPTPAN1 
NPDefPAN = NPTPAN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTPAN1 
NPDefPAN = NPTPAN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTPAN1 .c=Def1.c> =text> #NPTPAN1 
NPDefPAN = Pr1 NPTPAN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTPAN1 .c=Def1.c> =text> #NPTPAN1



MSPPPN = Pa1 Pa2 N1 <Pa1=Pa2=N1> (N1) =text> Pa1 Pa2 N1 <Pa1~>N1, Pa2~>N1>
TSPPN = MSPPPN1 ["\("MSPPPN2"\)"] <MSPPPN1.c=MSPPPN2.c> (MSPPPN1) =text> MSPPPN1
TPPN = TSPPN1 [[","] "���" ["������"] TSPPN2] <TSPPN1.c=TSPPN2.c> (TSPPN1) =text> TSPPN1

DefPPN = TPPN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPPN1
DefPPN = '���' TPPN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPPN1
DefPPN = TPPN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPPN1
DefPPN = TPPN1<c=ins> '��' '��������' Def1<c=acc> =text> #TPPN1
DefPPN = '���' TPPN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TPPN1
DefPPN = TPPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TPPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPPN1
DefPPN = TPPN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TPPN1
DefPPN = Def1<c=acc> ["��"] "�����" "��������" TPPN1<c=ins>  =text> #TPPN1
DefPPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TPPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPPN1
DefPPN = Def1<c=acc> "�����" "��������" TPPN1<c=ins>  =text> #TPPN1
DefPPN = Def1<c=acc> ["������"] "�������" TPPN1<c=ins> =text> #TPPN1
DefPPN = "�������" TPPN1<c=ins> Def1<c=acc>  =text> #TPPN1
DefPPN = "�������" Def1<c=acc> TPPN1<c=ins> =text> #TPPN1
DefPPN = "�������" Def1<c=acc> TPPN1<c=nom> =text> #TPPN1
DefPPN = Def1<c=acc> "," Pn1<�������> "�������" TPPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPPN1
DefPPN = Def1<c=acc> "�����" "�������" TPPN1<c=ins> =text> #TPPN1
DefPPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TPPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPPN1
DefPPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TPPN1<c=ins> =text> #TPPN1
DefPPN = "�����" "����" Pa1<�������, f=short> TPPN1<c=ins> <Pa1.n=TPPN1.n> =text> #TPPN1
DefPPN = TPPN1<c=ins> ["\("MSPPPN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TPPN1
DefPPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TPPN1<c=ins> <Def1.n=V1.n> =text> #TPPN1
DefPPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TPPN1<c=ins> <V1.n=Def1.n> =text> TPPN1
DefPPN = Def1<c=nom> "����������" ["�����" "���������" "-"] TPPN1<c=nom> =text> #TPPN1
DefPPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TPPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPPN1
DefPPN = TPPN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = Def1<c=acc> "�������" "��������" TPPN1<c=ins> =text> #TPPN1
DefPPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TPPN1<c=ins> =text> #TPPN1
DefPPN = TPPN1<c=ins> "��������" Def1<c=acc> =text> #TPPN1
DefPPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TPPN1<c=ins> <Def1=Pa1> =text> #TPPN1
DefPPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TPPN1<c=nom> <Pa1.n=N1.n> =text> #TPPN1
DefPPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPPN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPPN1
DefPPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPPN1<c=nom> <Def1.n=V1.n> =text> #TPPN1
DefPPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPPN1<c=gen> <Def1.n=V1.n> =text> #TPPN1
DefPPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPPN1<c=nom> <Def1.n=V1.n> =text> #TPPN1
DefPPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TPPN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPPN1
DefPPN = "���" Pa1<��������> TPPN1 <Pa1=TPPN1> =text> #TPPN1
DefPPN = {"�.�." | "�" "." "�" "."}<1,1> TPPN1 =text> #TPPN1
DefPPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TPPN1<c=ins> <Pa1=Def1>  =text> #TPPN1
DefPPN = Def1 "," Pa1<��������> TPPN1<c=nom> <Pa1=Def1> =text> #TPPN1
DefPPN = Def1 { "," | "\(" }<1,1> Pa1<�������> TPPN1<c=ins> <Def1=Pa1> =text> #TPPN1
DefPPN = Pn1 V1<��������, t=pres, p=3, m=ind> TPPN1<c=ins> <Pn1=V1> =text> #TPPN1
DefPPN = "���" TPPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "���" TPPN1<c=ins> "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "���" TPPN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "�����" "��������" "���" TPPN1<c=ins> Def1<c=acc> =text> #TPPN1
DefPPN = "������" "�������" TPPN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "������" "�������" TPPN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "���" "��������" TPPN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "���" TPPN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "���" TPPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TPPN1
DefPPN = "���" TPPN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPPN1
DefPPN = "���" "��������" TPPN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TPPN1.n=V1.n> =text> #TPPN1
DefPPN = N1<������> TPPN1<c=nom> =text> #TPPN1
DefPPN = "���" "������" TPPN1<c=nom> "����������" Def1<c=nom> =text> #TPPN1
DefPPN = TPPN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TPPN1
DefPPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TPPN1<c=gen> =text> #TPPN1
DefPPN = TPPN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TPPN1
DefPPN = TPPN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TPPN1.c=Def1.c> =text> #TPPN1
DefPPN = Pr1 TPPN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TPPN1.c=Def1.c> =text> #TPPN1

NPMSPPPN = Pa1 Pa2 N1 <Pa1=Pa2=N1> (N1) =text>"Pa1[" #Pa1 "] Pa2[" #Pa2 "] N1[" #N1 "] [Pa1=Pa2=N1] (N1) =text] Pa1 Pa2 N1 [Pa1~]N1, Pa2~]N1]"
NPTPPNSyn = NPMSPPPN1 ["\("NPMSPPPN2"\)"] <NPMSPPPN1.c=NPMSPPPN2.c> (NPMSPPPN1) =text> NPMSPPPN1
NPTPPN = NPTPPNSyn1 [[","] "���" ["������"] NPTPPNSyn2] <NPTPPNSyn1.c=NPTPPNSyn2.c> (NPTPPNSyn1) =text> NPTPPNSyn1

NPDefPPN = NPTPPN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPPN1 
NPDefPPN = '���' NPTPPN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTPPN1 
NPDefPPN = '���' NPTPPN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTPPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> ["��"] "�����" "��������" NPTPPN1 <c=ins>  =text> #NPTPPN1 
NPDefPPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTPPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> "�����" "��������" NPTPPN1 <c=ins>  =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> ["������"] "�������" NPTPPN1 <c=ins> =text> #NPTPPN1 
NPDefPPN = "�������" NPTPPN1 <c=ins> Def1<c=acc>  =text> #NPTPPN1 
NPDefPPN = "�������" Def1<c=acc> NPTPPN1 <c=ins> =text> #NPTPPN1 
NPDefPPN = "�������" Def1<c=acc> NPTPPN1 <c=nom> =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> "," Pn1<�������> "�������" NPTPPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> "�����" "�������" NPTPPN1 <c=ins> =text> #NPTPPN1 
NPDefPPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTPPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTPPN1 <c=ins> =text> #NPTPPN1 
NPDefPPN = "�����" "����" Pa1<�������, f=short> NPTPPN1 <c=ins> <Pa1.n=NPTPPN1 .n> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTPPN1 <c=ins> <Def1.n=V1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTPPN1 <c=ins> <V1.n=Def1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTPPN1 <c=nom> =text> #NPTPPN1 
NPDefPPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTPPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> "�������" "��������" NPTPPN1 <c=ins> =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTPPN1 <c=ins> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=ins> "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTPPN1 <c=ins> <Def1=Pa1> =text> #NPTPPN1 
NPDefPPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTPPN1 <c=nom> <Pa1.n=N1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPPN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPPN1 <c=nom> <Def1.n=V1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPPN1 <c=gen> <Def1.n=V1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPPN1 <c=nom> <Def1.n=V1.n> =text> #NPTPPN1 
NPDefPPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTPPN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPPN1 
NPDefPPN = "���" Pa1<��������> NPTPPN1  <Pa1=NPTPPN1 > =text> #NPTPPN1 
NPDefPPN = {"�.�." | "�" "." "�" "."}<1,1> NPTPPN1  =text> #NPTPPN1 
NPDefPPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTPPN1 <c=ins> <Pa1=Def1>  =text> #NPTPPN1 
NPDefPPN = Def1 "," Pa1<��������> NPTPPN1 <c=nom> <Pa1=Def1> =text> #NPTPPN1 
NPDefPPN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTPPN1 <c=ins> <Def1=Pa1> =text> #NPTPPN1 
NPDefPPN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTPPN1 <c=ins> <Pn1=V1> =text> #NPTPPN1 
NPDefPPN = "���" NPTPPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "���" NPTPPN1 <c=ins> "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "���" NPTPPN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "�����" "��������" "���" NPTPPN1 <c=ins> Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "������" "�������" NPTPPN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "������" "�������" NPTPPN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "���" "��������" NPTPPN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "���" NPTPPN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "���" NPTPPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPPN1 
NPDefPPN = "���" NPTPPN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPPN1 
NPDefPPN = "���" "��������" NPTPPN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTPPN1 .n=V1.n> =text> #NPTPPN1 
NPDefPPN = N1<������> NPTPPN1 <c=nom> =text> #NPTPPN1 
NPDefPPN = "���" "������" NPTPPN1 <c=nom> "����������" Def1<c=nom> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTPPN1 
NPDefPPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTPPN1 <c=gen> =text> #NPTPPN1 
NPDefPPN = NPTPPN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTPPN1 
NPDefPPN = NPTPPN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTPPN1 .c=Def1.c> =text> #NPTPPN1 
NPDefPPN = Pr1 NPTPPN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTPPN1 .c=Def1.c> =text> #NPTPPN1

MSPANN = A1 N1 N2<c=gen> <A1=N1> (N1) =text> A1 N1<A1~>N1> N2<c=gen>
TSANN = MSPANN1 ["\("MSPANN2"\)"] <MSPANN1.c=MSPANN2.c> (MSPANN1) =text> MSPANN1
TANNTWO = TSANN1 [[","] "���" ["������"] TSANN2] <TSANN1.c=TSANN2.c> (TSANN1) =text> TSANN1

DefANN = TANNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TANNTWO1
DefANN = '���' TANNTWO1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TANNTWO1
DefANN = TANNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TANNTWO1
DefANN = TANNTWO1<c=ins> '��' '��������' Def1<c=acc> =text> #TANNTWO1
DefANN = '���' TANNTWO1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TANNTWO1
DefANN = TANNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TANNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TANNTWO1
DefANN = TANNTWO1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TANNTWO1
DefANN = Def1<c=acc> ["��"] "�����" "��������" TANNTWO1<c=ins>  =text> #TANNTWO1
DefANN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TANNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TANNTWO1
DefANN = Def1<c=acc> "�����" "��������" TANNTWO1<c=ins>  =text> #TANNTWO1
DefANN = Def1<c=acc> ["������"] "�������" TANNTWO1<c=ins> =text> #TANNTWO1
DefANN = "�������" TANNTWO1<c=ins> Def1<c=acc>  =text> #TANNTWO1
DefANN = "�������" Def1<c=acc> TANNTWO1<c=ins> =text> #TANNTWO1
DefANN = "�������" Def1<c=acc> TANNTWO1<c=nom> =text> #TANNTWO1
DefANN = Def1<c=acc> "," Pn1<�������> "�������" TANNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TANNTWO1
DefANN = Def1<c=acc> "�����" "�������" TANNTWO1<c=ins> =text> #TANNTWO1
DefANN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TANNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TANNTWO1
DefANN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TANNTWO1<c=ins> =text> #TANNTWO1
DefANN = "�����" "����" Pa1<�������, f=short> TANNTWO1<c=ins> <Pa1.n=TANNTWO1.n> =text> #TANNTWO1
DefANN = TANNTWO1<c=ins> ["\("MSPANN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TANNTWO1
DefANN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TANNTWO1<c=ins> <Def1.n=V1.n> =text> #TANNTWO1
DefANN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TANNTWO1<c=ins> <V1.n=Def1.n> =text> TANNTWO1
DefANN = Def1<c=nom> "����������" ["�����" "���������" "-"] TANNTWO1<c=nom> =text> #TANNTWO1
DefANN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TANNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TANNTWO1
DefANN = TANNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = Def1<c=acc> "�������" "��������" TANNTWO1<c=ins> =text> #TANNTWO1
DefANN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TANNTWO1<c=ins> =text> #TANNTWO1
DefANN = TANNTWO1<c=ins> "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TANNTWO1<c=ins> <Def1=Pa1> =text> #TANNTWO1
DefANN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TANNTWO1<c=nom> <Pa1.n=N1.n> =text> #TANNTWO1
DefANN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TANNTWO1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TANNTWO1
DefANN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TANNTWO1<c=nom> <Def1.n=V1.n> =text> #TANNTWO1
DefANN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TANNTWO1<c=gen> <Def1.n=V1.n> =text> #TANNTWO1
DefANN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TANNTWO1<c=nom> <Def1.n=V1.n> =text> #TANNTWO1
DefANN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TANNTWO1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TANNTWO1
DefANN = "���" Pa1<��������> TANNTWO1 <Pa1=TANNTWO1> =text> #TANNTWO1
DefANN = {"�.�." | "�" "." "�" "."}<1,1> TANNTWO1 =text> #TANNTWO1
DefANN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TANNTWO1<c=ins> <Pa1=Def1>  =text> #TANNTWO1
DefANN = Def1 "," Pa1<��������> TANNTWO1<c=nom> <Pa1=Def1> =text> #TANNTWO1
DefANN = Def1 { "," | "\(" }<1,1> Pa1<�������> TANNTWO1<c=ins> <Def1=Pa1> =text> #TANNTWO1
DefANN = Pn1 V1<��������, t=pres, p=3, m=ind> TANNTWO1<c=ins> <Pn1=V1> =text> #TANNTWO1
DefANN = "���" TANNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "���" TANNTWO1<c=ins> "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "���" TANNTWO1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "�����" "��������" "���" TANNTWO1<c=ins> Def1<c=acc> =text> #TANNTWO1
DefANN = "������" "�������" TANNTWO1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "������" "�������" TANNTWO1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "���" "��������" TANNTWO1<c=nom> "�����" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "���" TANNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "���" TANNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TANNTWO1
DefANN = "���" TANNTWO1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TANNTWO1
DefANN = "���" "��������" TANNTWO1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TANNTWO1.n=V1.n> =text> #TANNTWO1
DefANN = N1<������> TANNTWO1<c=nom> =text> #TANNTWO1
DefANN = "���" "������" TANNTWO1<c=nom> "����������" Def1<c=nom> =text> #TANNTWO1
DefANN = TANNTWO1<c=nom> ["�"] "����" Def1<c=nom> =text> #TANNTWO1
DefANN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TANNTWO1<c=gen> =text> #TANNTWO1
DefANN = TANNTWO1<c=nom> ["�"] "���" Def1<c=nom> =text> #TANNTWO1
DefANN = TANNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TANNTWO1.c=Def1.c> =text> #TANNTWO1
DefANN = Pr1 TANNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TANNTWO1.c=Def1.c> =text> #TANNTWO1

NPMSPANN = A1 N1 N2<c=gen> <A1=N1> (N1) =text> "A1[" #A1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [A1=N1] (N1) =text] A1 N1[A1~]N1] N2[c=gen]"
NPTANNTWOSyn = NPMSPANN1 ["\("NPMSPANN2"\)"] <NPMSPANN1.c=NPMSPANN2.c> (NPMSPANN1) =text> NPMSPANN1
NPTANNTWO = NPTANNTWOSyn1 [[","] "���" ["������"] NPTANNTWOSyn2] <NPTANNTWOSyn1.c=NPTANNTWOSyn2.c> (NPTANNTWOSyn1) =text> NPTANNTWOSyn1

NPDefANN = NPTANNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTANNTWO1 
NPDefANN = '���' NPTANNTWO1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=ins> '��' '��������' Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = '���' NPTANNTWO1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTANNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> ["��"] "�����" "��������" NPTANNTWO1 <c=ins>  =text> #NPTANNTWO1 
NPDefANN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTANNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> "�����" "��������" NPTANNTWO1 <c=ins>  =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> ["������"] "�������" NPTANNTWO1 <c=ins> =text> #NPTANNTWO1 
NPDefANN = "�������" NPTANNTWO1 <c=ins> Def1<c=acc>  =text> #NPTANNTWO1 
NPDefANN = "�������" Def1<c=acc> NPTANNTWO1 <c=ins> =text> #NPTANNTWO1 
NPDefANN = "�������" Def1<c=acc> NPTANNTWO1 <c=nom> =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> "," Pn1<�������> "�������" NPTANNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> "�����" "�������" NPTANNTWO1 <c=ins> =text> #NPTANNTWO1 
NPDefANN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTANNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTANNTWO1 <c=ins> =text> #NPTANNTWO1 
NPDefANN = "�����" "����" Pa1<�������, f=short> NPTANNTWO1 <c=ins> <Pa1.n=NPTANNTWO1 .n> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTANNTWO1 <c=ins> <Def1.n=V1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTANNTWO1 <c=ins> <V1.n=Def1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTANNTWO1 <c=nom> =text> #NPTANNTWO1 
NPDefANN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTANNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> "�������" "��������" NPTANNTWO1 <c=ins> =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTANNTWO1 <c=ins> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTANNTWO1 <c=ins> <Def1=Pa1> =text> #NPTANNTWO1 
NPDefANN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTANNTWO1 <c=nom> <Pa1.n=N1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTANNTWO1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTANNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTANNTWO1 <c=gen> <Def1.n=V1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTANNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTANNTWO1 
NPDefANN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTANNTWO1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTANNTWO1 
NPDefANN = "���" Pa1<��������> NPTANNTWO1  <Pa1=NPTANNTWO1 > =text> #NPTANNTWO1 
NPDefANN = {"�.�." | "�" "." "�" "."}<1,1> NPTANNTWO1  =text> #NPTANNTWO1 
NPDefANN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTANNTWO1 <c=ins> <Pa1=Def1>  =text> #NPTANNTWO1 
NPDefANN = Def1 "," Pa1<��������> NPTANNTWO1 <c=nom> <Pa1=Def1> =text> #NPTANNTWO1 
NPDefANN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTANNTWO1 <c=ins> <Def1=Pa1> =text> #NPTANNTWO1 
NPDefANN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTANNTWO1 <c=ins> <Pn1=V1> =text> #NPTANNTWO1 
NPDefANN = "���" NPTANNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "���" NPTANNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "���" NPTANNTWO1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "�����" "��������" "���" NPTANNTWO1 <c=ins> Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "������" "�������" NPTANNTWO1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "������" "�������" NPTANNTWO1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "���" "��������" NPTANNTWO1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "���" NPTANNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "���" NPTANNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTANNTWO1 
NPDefANN = "���" NPTANNTWO1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTANNTWO1 
NPDefANN = "���" "��������" NPTANNTWO1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTANNTWO1 .n=V1.n> =text> #NPTANNTWO1 
NPDefANN = N1<������> NPTANNTWO1 <c=nom> =text> #NPTANNTWO1 
NPDefANN = "���" "������" NPTANNTWO1 <c=nom> "����������" Def1<c=nom> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTANNTWO1 
NPDefANN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTANNTWO1 <c=gen> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTANNTWO1 
NPDefANN = NPTANNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTANNTWO1 .c=Def1.c> =text> #NPTANNTWO1 
NPDefANN = Pr1 NPTANNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTANNTWO1 .c=Def1.c> =text> #NPTANNTWO1



MSPPNN = Pa1 N1 N2<c=gen> <Pa1=N1> (N1) =text> Pa1 N1<Pa1~>N1> N2<c=gen>
TSPNN = MSPPNN1 ["\("MSPPNN2"\)"] <MSPPNN1.c=MSPPNN2.c> (MSPPNN1) =text> MSPPNN1
TPNNTWO = TSPNN1 [[","] "���" ["������"] TSPNN2] <TSPNN1.c=TSPNN2.c> (TSPNN1) =text> TSPNN1

DefPNN = TPNNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPNNTWO1
DefPNN = '���' TPNNTWO1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=ins> '��' '��������' Def1<c=acc> =text> #TPNNTWO1
DefPNN = '���' TPNNTWO1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TPNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TPNNTWO1
DefPNN = Def1<c=acc> ["��"] "�����" "��������" TPNNTWO1<c=ins>  =text> #TPNNTWO1
DefPNN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TPNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TPNNTWO1
DefPNN = Def1<c=acc> "�����" "��������" TPNNTWO1<c=ins>  =text> #TPNNTWO1
DefPNN = Def1<c=acc> ["������"] "�������" TPNNTWO1<c=ins> =text> #TPNNTWO1
DefPNN = "�������" TPNNTWO1<c=ins> Def1<c=acc>  =text> #TPNNTWO1
DefPNN = "�������" Def1<c=acc> TPNNTWO1<c=ins> =text> #TPNNTWO1
DefPNN = "�������" Def1<c=acc> TPNNTWO1<c=nom> =text> #TPNNTWO1
DefPNN = Def1<c=acc> "," Pn1<�������> "�������" TPNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPNNTWO1
DefPNN = Def1<c=acc> "�����" "�������" TPNNTWO1<c=ins> =text> #TPNNTWO1
DefPNN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TPNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPNNTWO1
DefPNN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TPNNTWO1<c=ins> =text> #TPNNTWO1
DefPNN = "�����" "����" Pa1<�������, f=short> TPNNTWO1<c=ins> <Pa1.n=TPNNTWO1.n> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=ins> ["\("MSPPNN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TPNNTWO1
DefPNN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TPNNTWO1<c=ins> <Def1.n=V1.n> =text> #TPNNTWO1
DefPNN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TPNNTWO1<c=ins> <V1.n=Def1.n> =text> TPNNTWO1
DefPNN = Def1<c=nom> "����������" ["�����" "���������" "-"] TPNNTWO1<c=nom> =text> #TPNNTWO1
DefPNN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TPNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = Def1<c=acc> "�������" "��������" TPNNTWO1<c=ins> =text> #TPNNTWO1
DefPNN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TPNNTWO1<c=ins> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=ins> "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TPNNTWO1<c=ins> <Def1=Pa1> =text> #TPNNTWO1
DefPNN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TPNNTWO1<c=nom> <Pa1.n=N1.n> =text> #TPNNTWO1
DefPNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPNNTWO1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPNNTWO1
DefPNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TPNNTWO1<c=nom> <Def1.n=V1.n> =text> #TPNNTWO1
DefPNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPNNTWO1<c=gen> <Def1.n=V1.n> =text> #TPNNTWO1
DefPNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TPNNTWO1<c=nom> <Def1.n=V1.n> =text> #TPNNTWO1
DefPNN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TPNNTWO1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TPNNTWO1
DefPNN = "���" Pa1<��������> TPNNTWO1 <Pa1=TPNNTWO1> =text> #TPNNTWO1
DefPNN = {"�.�." | "�" "." "�" "."}<1,1> TPNNTWO1 =text> #TPNNTWO1
DefPNN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TPNNTWO1<c=ins> <Pa1=Def1>  =text> #TPNNTWO1
DefPNN = Def1 "," Pa1<��������> TPNNTWO1<c=nom> <Pa1=Def1> =text> #TPNNTWO1
DefPNN = Def1 { "," | "\(" }<1,1> Pa1<�������> TPNNTWO1<c=ins> <Def1=Pa1> =text> #TPNNTWO1
DefPNN = Pn1 V1<��������, t=pres, p=3, m=ind> TPNNTWO1<c=ins> <Pn1=V1> =text> #TPNNTWO1
DefPNN = "���" TPNNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "���" TPNNTWO1<c=ins> "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "���" TPNNTWO1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "�����" "��������" "���" TPNNTWO1<c=ins> Def1<c=acc> =text> #TPNNTWO1
DefPNN = "������" "�������" TPNNTWO1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "������" "�������" TPNNTWO1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "���" "��������" TPNNTWO1<c=nom> "�����" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "���" TPNNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "���" TPNNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TPNNTWO1
DefPNN = "���" TPNNTWO1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TPNNTWO1
DefPNN = "���" "��������" TPNNTWO1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TPNNTWO1.n=V1.n> =text> #TPNNTWO1
DefPNN = N1<������> TPNNTWO1<c=nom> =text> #TPNNTWO1
DefPNN = "���" "������" TPNNTWO1<c=nom> "����������" Def1<c=nom> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=nom> ["�"] "����" Def1<c=nom> =text> #TPNNTWO1
DefPNN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TPNNTWO1<c=gen> =text> #TPNNTWO1
DefPNN = TPNNTWO1<c=nom> ["�"] "���" Def1<c=nom> =text> #TPNNTWO1
DefPNN = TPNNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TPNNTWO1.c=Def1.c> =text> #TPNNTWO1
DefPNN = Pr1 TPNNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TPNNTWO1.c=Def1.c> =text> #TPNNTWO1

NPMSPPNN = Pa1 N1 N2<c=gen> <Pa1=N1> (N1) =text> "Pa1[" #Pa1 "] N1[" #N1 "] N2[" #N2 ",c=gen] [Pa1=N1] (N1) =text] Pa1 N1[Pa1~]N1] N2[c=gen]"
NPTPNNTWOSyn = NPMSPPNN1 ["\("NPMSPPNN2"\)"] <NPMSPPNN1.c=NPMSPPNN2.c> (NPMSPPNN1) =text> NPMSPPNN1
NPTPNNTWO = NPTPNNTWOSyn1 [[","] "���" ["������"] NPTPNNTWOSyn2] <NPTPNNTWOSyn1.c=NPTPNNTWOSyn2.c> (NPTPNNTWOSyn1) =text> NPTPNNTWOSyn1

NPDefPNN = NPTPNNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPNNTWO1 
NPDefPNN = '���' NPTPNNTWO1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=ins> '��' '��������' Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = '���' NPTPNNTWO1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTPNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> ["��"] "�����" "��������" NPTPNNTWO1 <c=ins>  =text> #NPTPNNTWO1 
NPDefPNN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTPNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> "�����" "��������" NPTPNNTWO1 <c=ins>  =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> ["������"] "�������" NPTPNNTWO1 <c=ins> =text> #NPTPNNTWO1 
NPDefPNN = "�������" NPTPNNTWO1 <c=ins> Def1<c=acc>  =text> #NPTPNNTWO1 
NPDefPNN = "�������" Def1<c=acc> NPTPNNTWO1 <c=ins> =text> #NPTPNNTWO1 
NPDefPNN = "�������" Def1<c=acc> NPTPNNTWO1 <c=nom> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> "," Pn1<�������> "�������" NPTPNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> "�����" "�������" NPTPNNTWO1 <c=ins> =text> #NPTPNNTWO1 
NPDefPNN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTPNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTPNNTWO1 <c=ins> =text> #NPTPNNTWO1 
NPDefPNN = "�����" "����" Pa1<�������, f=short> NPTPNNTWO1 <c=ins> <Pa1.n=NPTPNNTWO1 .n> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTPNNTWO1 <c=ins> <Def1.n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTPNNTWO1 <c=ins> <V1.n=Def1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTPNNTWO1 <c=nom> =text> #NPTPNNTWO1 
NPDefPNN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTPNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> "�������" "��������" NPTPNNTWO1 <c=ins> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTPNNTWO1 <c=ins> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTPNNTWO1 <c=ins> <Def1=Pa1> =text> #NPTPNNTWO1 
NPDefPNN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTPNNTWO1 <c=nom> <Pa1.n=N1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPNNTWO1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTPNNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPNNTWO1 <c=gen> <Def1.n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTPNNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTPNNTWO1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTPNNTWO1 
NPDefPNN = "���" Pa1<��������> NPTPNNTWO1  <Pa1=NPTPNNTWO1 > =text> #NPTPNNTWO1 
NPDefPNN = {"�.�." | "�" "." "�" "."}<1,1> NPTPNNTWO1  =text> #NPTPNNTWO1 
NPDefPNN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTPNNTWO1 <c=ins> <Pa1=Def1>  =text> #NPTPNNTWO1 
NPDefPNN = Def1 "," Pa1<��������> NPTPNNTWO1 <c=nom> <Pa1=Def1> =text> #NPTPNNTWO1 
NPDefPNN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTPNNTWO1 <c=ins> <Def1=Pa1> =text> #NPTPNNTWO1 
NPDefPNN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTPNNTWO1 <c=ins> <Pn1=V1> =text> #NPTPNNTWO1 
NPDefPNN = "���" NPTPNNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "���" NPTPNNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "���" NPTPNNTWO1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "�����" "��������" "���" NPTPNNTWO1 <c=ins> Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "������" "�������" NPTPNNTWO1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "������" "�������" NPTPNNTWO1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "���" "��������" NPTPNNTWO1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "���" NPTPNNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "���" NPTPNNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTPNNTWO1 
NPDefPNN = "���" NPTPNNTWO1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = "���" "��������" NPTPNNTWO1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTPNNTWO1 .n=V1.n> =text> #NPTPNNTWO1 
NPDefPNN = N1<������> NPTPNNTWO1 <c=nom> =text> #NPTPNNTWO1 
NPDefPNN = "���" "������" NPTPNNTWO1 <c=nom> "����������" Def1<c=nom> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTPNNTWO1 
NPDefPNN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTPNNTWO1 <c=gen> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTPNNTWO1 
NPDefPNN = NPTPNNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTPNNTWO1 .c=Def1.c> =text> #NPTPNNTWO1 
NPDefPNN = Pr1 NPTPNNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTPNNTWO1 .c=Def1.c> =text> #NPTPNNTWO1

MSPNAN = N1 A1 N2<c=gen> <A1=N2> (N1) =text> N1 A1 N2<c=gen><A1~>N2>
TSNAN = MSPNAN1 ["\("MSPNAN2"\)"] <MSPNAN1.c=MSPNAN2.c> (MSPNAN1) =text> MSPNAN1
TNAN = TSNAN1 [[","] "���" ["������"] TSNAN2] <TSNAN1.c=TSNAN2.c> (TSNAN1) =text> TSNAN1

DefNAN = TNAN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNAN1
DefNAN = '���' TNAN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNAN1
DefNAN = TNAN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNAN1
DefNAN = TNAN1<c=ins> '��' '��������' Def1<c=acc> =text> #TNAN1
DefNAN = '���' TNAN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TNAN1
DefNAN = TNAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TNAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNAN1
DefNAN = TNAN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TNAN1
DefNAN = Def1<c=acc> ["��"] "�����" "��������" TNAN1<c=ins>  =text> #TNAN1
DefNAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TNAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNAN1
DefNAN = Def1<c=acc> "�����" "��������" TNAN1<c=ins>  =text> #TNAN1
DefNAN = Def1<c=acc> ["������"] "�������" TNAN1<c=ins> =text> #TNAN1
DefNAN = "�������" TNAN1<c=ins> Def1<c=acc>  =text> #TNAN1
DefNAN = "�������" Def1<c=acc> TNAN1<c=ins> =text> #TNAN1
DefNAN = "�������" Def1<c=acc> TNAN1<c=nom> =text> #TNAN1
DefNAN = Def1<c=acc> "," Pn1<�������> "�������" TNAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNAN1
DefNAN = Def1<c=acc> "�����" "�������" TNAN1<c=ins> =text> #TNAN1
DefNAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TNAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNAN1
DefNAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TNAN1<c=ins> =text> #TNAN1
DefNAN = "�����" "����" Pa1<�������, f=short> TNAN1<c=ins> <Pa1.n=TNAN1.n> =text> #TNAN1
DefNAN = TNAN1<c=ins> ["\("MSPNAN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TNAN1
DefNAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TNAN1<c=ins> <Def1.n=V1.n> =text> #TNAN1
DefNAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TNAN1<c=ins> <V1.n=Def1.n> =text> TNAN1
DefNAN = Def1<c=nom> "����������" ["�����" "���������" "-"] TNAN1<c=nom> =text> #TNAN1
DefNAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TNAN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNAN1
DefNAN = TNAN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = Def1<c=acc> "�������" "��������" TNAN1<c=ins> =text> #TNAN1
DefNAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TNAN1<c=ins> =text> #TNAN1
DefNAN = TNAN1<c=ins> "��������" Def1<c=acc> =text> #TNAN1
DefNAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TNAN1<c=ins> <Def1=Pa1> =text> #TNAN1
DefNAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TNAN1<c=nom> <Pa1.n=N1.n> =text> #TNAN1
DefNAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNAN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNAN1
DefNAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNAN1<c=nom> <Def1.n=V1.n> =text> #TNAN1
DefNAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNAN1<c=gen> <Def1.n=V1.n> =text> #TNAN1
DefNAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNAN1<c=nom> <Def1.n=V1.n> =text> #TNAN1
DefNAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TNAN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNAN1
DefNAN = "���" Pa1<��������> TNAN1 <Pa1=TNAN1> =text> #TNAN1
DefNAN = {"�.�." | "�" "." "�" "."}<1,1> TNAN1 =text> #TNAN1
DefNAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TNAN1<c=ins> <Pa1=Def1>  =text> #TNAN1
DefNAN = Def1 "," Pa1<��������> TNAN1<c=nom> <Pa1=Def1> =text> #TNAN1
DefNAN = Def1 { "," | "\(" }<1,1> Pa1<�������> TNAN1<c=ins> <Def1=Pa1> =text> #TNAN1
DefNAN = Pn1 V1<��������, t=pres, p=3, m=ind> TNAN1<c=ins> <Pn1=V1> =text> #TNAN1
DefNAN = "���" TNAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "���" TNAN1<c=ins> "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "���" TNAN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "�����" "��������" "���" TNAN1<c=ins> Def1<c=acc> =text> #TNAN1
DefNAN = "������" "�������" TNAN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "������" "�������" TNAN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "���" "��������" TNAN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "���" TNAN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "���" TNAN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNAN1
DefNAN = "���" TNAN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNAN1
DefNAN = "���" "��������" TNAN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TNAN1.n=V1.n> =text> #TNAN1
DefNAN = N1<������> TNAN1<c=nom> =text> #TNAN1
DefNAN = "���" "������" TNAN1<c=nom> "����������" Def1<c=nom> =text> #TNAN1
DefNAN = TNAN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TNAN1
DefNAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TNAN1<c=gen> =text> #TNAN1
DefNAN = TNAN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TNAN1
DefNAN = TNAN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TNAN1.c=Def1.c> =text> #TNAN1
DefNAN = Pr1 TNAN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TNAN1.c=Def1.c> =text> #TNAN1

NPMSPNAN = N1 A1 N2<c=gen> <A1=N2> (N1) =text> "N1[" #N1 "] A1[" #A1 "] N2[" #N2 ",c=gen] [A1=N2] (N1) =text] N1 A1 N2[c=gen][A1~]N2]"
NPTNANSyn = NPMSPNAN1 ["\("NPMSPNAN2"\)"] <NPMSPNAN1.c=NPMSPNAN2.c> (NPMSPNAN1) =text> NPMSPNAN1
NPTNAN = NPTNANSyn1 [[","] "���" ["������"] NPTNANSyn2] <NPTNANSyn1.c=NPTNANSyn2.c> (NPTNANSyn1) =text> NPTNANSyn1

NPDefNAN = NPTNAN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNAN1 
NPDefNAN = '���' NPTNAN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTNAN1 
NPDefNAN = '���' NPTNAN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTNAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> ["��"] "�����" "��������" NPTNAN1 <c=ins>  =text> #NPTNAN1 
NPDefNAN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTNAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> "�����" "��������" NPTNAN1 <c=ins>  =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> ["������"] "�������" NPTNAN1 <c=ins> =text> #NPTNAN1 
NPDefNAN = "�������" NPTNAN1 <c=ins> Def1<c=acc>  =text> #NPTNAN1 
NPDefNAN = "�������" Def1<c=acc> NPTNAN1 <c=ins> =text> #NPTNAN1 
NPDefNAN = "�������" Def1<c=acc> NPTNAN1 <c=nom> =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> "," Pn1<�������> "�������" NPTNAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> "�����" "�������" NPTNAN1 <c=ins> =text> #NPTNAN1 
NPDefNAN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTNAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTNAN1 <c=ins> =text> #NPTNAN1 
NPDefNAN = "�����" "����" Pa1<�������, f=short> NPTNAN1 <c=ins> <Pa1.n=NPTNAN1 .n> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTNAN1 <c=ins> <Def1.n=V1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTNAN1 <c=ins> <V1.n=Def1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTNAN1 <c=nom> =text> #NPTNAN1 
NPDefNAN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTNAN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> "�������" "��������" NPTNAN1 <c=ins> =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTNAN1 <c=ins> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=ins> "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTNAN1 <c=ins> <Def1=Pa1> =text> #NPTNAN1 
NPDefNAN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTNAN1 <c=nom> <Pa1.n=N1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNAN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNAN1 <c=nom> <Def1.n=V1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNAN1 <c=gen> <Def1.n=V1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNAN1 <c=nom> <Def1.n=V1.n> =text> #NPTNAN1 
NPDefNAN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTNAN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNAN1 
NPDefNAN = "���" Pa1<��������> NPTNAN1  <Pa1=NPTNAN1 > =text> #NPTNAN1 
NPDefNAN = {"�.�." | "�" "." "�" "."}<1,1> NPTNAN1  =text> #NPTNAN1 
NPDefNAN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTNAN1 <c=ins> <Pa1=Def1>  =text> #NPTNAN1 
NPDefNAN = Def1 "," Pa1<��������> NPTNAN1 <c=nom> <Pa1=Def1> =text> #NPTNAN1 
NPDefNAN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTNAN1 <c=ins> <Def1=Pa1> =text> #NPTNAN1 
NPDefNAN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTNAN1 <c=ins> <Pn1=V1> =text> #NPTNAN1 
NPDefNAN = "���" NPTNAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "���" NPTNAN1 <c=ins> "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "���" NPTNAN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "�����" "��������" "���" NPTNAN1 <c=ins> Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "������" "�������" NPTNAN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "������" "�������" NPTNAN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "���" "��������" NPTNAN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "���" NPTNAN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "���" NPTNAN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNAN1 
NPDefNAN = "���" NPTNAN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNAN1 
NPDefNAN = "���" "��������" NPTNAN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTNAN1 .n=V1.n> =text> #NPTNAN1 
NPDefNAN = N1<������> NPTNAN1 <c=nom> =text> #NPTNAN1 
NPDefNAN = "���" "������" NPTNAN1 <c=nom> "����������" Def1<c=nom> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTNAN1 
NPDefNAN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTNAN1 <c=gen> =text> #NPTNAN1 
NPDefNAN = NPTNAN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTNAN1 
NPDefNAN = NPTNAN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTNAN1 .c=Def1.c> =text> #NPTNAN1 
NPDefNAN = Pr1 NPTNAN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTNAN1 .c=Def1.c> =text> #NPTNAN1


MSPNPN = N1 Pa1 N2<c=gen> <Pa1=N2> (N1) =text> N1 Pa1 N2<c=gen><Pa1~>N2>
TSNPN = MSPNPN1 ["\("MSPNPN2"\)"] <MSPNPN1.c=MSPNPN2.c> (MSPNPN1) =text> MSPNPN1
TNPN = TSNPN1 [[","] "���" ["������"] TSNPN2] <TSNPN1.c=TSNPN2.c> (TSNPN1) =text> TSNPN1

DefNPN = TNPN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNPN1
DefNPN = '���' TNPN1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNPN1
DefNPN = TNPN1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNPN1
DefNPN = TNPN1<c=ins> '��' '��������' Def1<c=acc> =text> #TNPN1
DefNPN = '���' TNPN1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TNPN1
DefNPN = TNPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TNPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNPN1
DefNPN = TNPN1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TNPN1
DefNPN = Def1<c=acc> ["��"] "�����" "��������" TNPN1<c=ins>  =text> #TNPN1
DefNPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TNPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNPN1
DefNPN = Def1<c=acc> "�����" "��������" TNPN1<c=ins>  =text> #TNPN1
DefNPN = Def1<c=acc> ["������"] "�������" TNPN1<c=ins> =text> #TNPN1
DefNPN = "�������" TNPN1<c=ins> Def1<c=acc>  =text> #TNPN1
DefNPN = "�������" Def1<c=acc> TNPN1<c=ins> =text> #TNPN1
DefNPN = "�������" Def1<c=acc> TNPN1<c=nom> =text> #TNPN1
DefNPN = Def1<c=acc> "," Pn1<�������> "�������" TNPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNPN1
DefNPN = Def1<c=acc> "�����" "�������" TNPN1<c=ins> =text> #TNPN1
DefNPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TNPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNPN1
DefNPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TNPN1<c=ins> =text> #TNPN1
DefNPN = "�����" "����" Pa1<�������, f=short> TNPN1<c=ins> <Pa1.n=TNPN1.n> =text> #TNPN1
DefNPN = TNPN1<c=ins> ["\("MSPNPN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TNPN1
DefNPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TNPN1<c=ins> <Def1.n=V1.n> =text> #TNPN1
DefNPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TNPN1<c=ins> <V1.n=Def1.n> =text> TNPN1
DefNPN = Def1<c=nom> "����������" ["�����" "���������" "-"] TNPN1<c=nom> =text> #TNPN1
DefNPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TNPN1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNPN1
DefNPN = TNPN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = Def1<c=acc> "�������" "��������" TNPN1<c=ins> =text> #TNPN1
DefNPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TNPN1<c=ins> =text> #TNPN1
DefNPN = TNPN1<c=ins> "��������" Def1<c=acc> =text> #TNPN1
DefNPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TNPN1<c=ins> <Def1=Pa1> =text> #TNPN1
DefNPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TNPN1<c=nom> <Pa1.n=N1.n> =text> #TNPN1
DefNPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNPN1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNPN1
DefNPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNPN1<c=nom> <Def1.n=V1.n> =text> #TNPN1
DefNPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNPN1<c=gen> <Def1.n=V1.n> =text> #TNPN1
DefNPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNPN1<c=nom> <Def1.n=V1.n> =text> #TNPN1
DefNPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TNPN1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNPN1
DefNPN = "���" Pa1<��������> TNPN1 <Pa1=TNPN1> =text> #TNPN1
DefNPN = {"�.�." | "�" "." "�" "."}<1,1> TNPN1 =text> #TNPN1
DefNPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TNPN1<c=ins> <Pa1=Def1>  =text> #TNPN1
DefNPN = Def1 "," Pa1<��������> TNPN1<c=nom> <Pa1=Def1> =text> #TNPN1
DefNPN = Def1 { "," | "\(" }<1,1> Pa1<�������> TNPN1<c=ins> <Def1=Pa1> =text> #TNPN1
DefNPN = Pn1 V1<��������, t=pres, p=3, m=ind> TNPN1<c=ins> <Pn1=V1> =text> #TNPN1
DefNPN = "���" TNPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "���" TNPN1<c=ins> "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "���" TNPN1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "�����" "��������" "���" TNPN1<c=ins> Def1<c=acc> =text> #TNPN1
DefNPN = "������" "�������" TNPN1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "������" "�������" TNPN1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "���" "��������" TNPN1<c=nom> "�����" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "���" TNPN1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "���" TNPN1<c=ins> "��" "��������" Def1<c=acc> =text> #TNPN1
DefNPN = "���" TNPN1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNPN1
DefNPN = "���" "��������" TNPN1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TNPN1.n=V1.n> =text> #TNPN1
DefNPN = N1<������> TNPN1<c=nom> =text> #TNPN1
DefNPN = "���" "������" TNPN1<c=nom> "����������" Def1<c=nom> =text> #TNPN1
DefNPN = TNPN1<c=nom> ["�"] "����" Def1<c=nom> =text> #TNPN1
DefNPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TNPN1<c=gen> =text> #TNPN1
DefNPN = TNPN1<c=nom> ["�"] "���" Def1<c=nom> =text> #TNPN1
DefNPN = TNPN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TNPN1.c=Def1.c> =text> #TNPN1
DefNPN = Pr1 TNPN1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TNPN1.c=Def1.c> =text> #TNPN1

NPMSPNPN = N1 Pa1 N2<c=gen> <Pa1=N2> (N1) =text> "N1[" #N1 "] Pa1[" #Pa1 "] N2[" #N2 ",c=gen] [Pa1=N2] (N1) =text] N1 Pa1 N2[c=gen][Pa1~]N2]"
NPTNPNSyn = NPMSPNPN1 ["\("NPMSPNPN2"\)"] <NPMSPNPN1.c=NPMSPNPN2.c> (NPMSPNPN1) =text> NPMSPNPN1
NPTNPN = NPTNPNSyn1 [[","] "���" ["������"] NPTNPNSyn2] <NPTNPNSyn1.c=NPTNPNSyn2.c> (NPTNPNSyn1) =text> NPTNPNSyn1

NPDefNPN = NPTNPN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNPN1 
NPDefNPN = '���' NPTNPN1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTNPN1 
NPDefNPN = '���' NPTNPN1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTNPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> ["��"] "�����" "��������" NPTNPN1 <c=ins>  =text> #NPTNPN1 
NPDefNPN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTNPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> "�����" "��������" NPTNPN1 <c=ins>  =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> ["������"] "�������" NPTNPN1 <c=ins> =text> #NPTNPN1 
NPDefNPN = "�������" NPTNPN1 <c=ins> Def1<c=acc>  =text> #NPTNPN1 
NPDefNPN = "�������" Def1<c=acc> NPTNPN1 <c=ins> =text> #NPTNPN1 
NPDefNPN = "�������" Def1<c=acc> NPTNPN1 <c=nom> =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> "," Pn1<�������> "�������" NPTNPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> "�����" "�������" NPTNPN1 <c=ins> =text> #NPTNPN1 
NPDefNPN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTNPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTNPN1 <c=ins> =text> #NPTNPN1 
NPDefNPN = "�����" "����" Pa1<�������, f=short> NPTNPN1 <c=ins> <Pa1.n=NPTNPN1 .n> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTNPN1 <c=ins> <Def1.n=V1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTNPN1 <c=ins> <V1.n=Def1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTNPN1 <c=nom> =text> #NPTNPN1 
NPDefNPN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTNPN1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> "�������" "��������" NPTNPN1 <c=ins> =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTNPN1 <c=ins> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=ins> "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTNPN1 <c=ins> <Def1=Pa1> =text> #NPTNPN1 
NPDefNPN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTNPN1 <c=nom> <Pa1.n=N1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNPN1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNPN1 <c=nom> <Def1.n=V1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNPN1 <c=gen> <Def1.n=V1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNPN1 <c=nom> <Def1.n=V1.n> =text> #NPTNPN1 
NPDefNPN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTNPN1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNPN1 
NPDefNPN = "���" Pa1<��������> NPTNPN1  <Pa1=NPTNPN1 > =text> #NPTNPN1 
NPDefNPN = {"�.�." | "�" "." "�" "."}<1,1> NPTNPN1  =text> #NPTNPN1 
NPDefNPN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTNPN1 <c=ins> <Pa1=Def1>  =text> #NPTNPN1 
NPDefNPN = Def1 "," Pa1<��������> NPTNPN1 <c=nom> <Pa1=Def1> =text> #NPTNPN1 
NPDefNPN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTNPN1 <c=ins> <Def1=Pa1> =text> #NPTNPN1 
NPDefNPN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTNPN1 <c=ins> <Pn1=V1> =text> #NPTNPN1 
NPDefNPN = "���" NPTNPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "���" NPTNPN1 <c=ins> "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "���" NPTNPN1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "�����" "��������" "���" NPTNPN1 <c=ins> Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "������" "�������" NPTNPN1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "������" "�������" NPTNPN1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "���" "��������" NPTNPN1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "���" NPTNPN1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "���" NPTNPN1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNPN1 
NPDefNPN = "���" NPTNPN1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNPN1 
NPDefNPN = "���" "��������" NPTNPN1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTNPN1 .n=V1.n> =text> #NPTNPN1 
NPDefNPN = N1<������> NPTNPN1 <c=nom> =text> #NPTNPN1 
NPDefNPN = "���" "������" NPTNPN1 <c=nom> "����������" Def1<c=nom> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTNPN1 
NPDefNPN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTNPN1 <c=gen> =text> #NPTNPN1 
NPDefNPN = NPTNPN1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTNPN1 
NPDefNPN = NPTNPN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTNPN1 .c=Def1.c> =text> #NPTNPN1 
NPDefNPN = Pr1 NPTNPN1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTNPN1 .c=Def1.c> =text> #NPTNPN1


MSPNNN = N1 N2<c=gen> N3<c=gen> (N1) =text> N1 N2<c=gen> N3<c=gen>
TSNNN = MSPNNN1 ["\("MSPNNN2"\)"] <MSPNNN1.c=MSPNNN2.c> (MSPNNN1) =text> MSPNNN1
TNNNTWO = TSNNN1 [[","] "���" ["������"] TSNNN2] <TSNNN1.c=TSNNN2.c> (TSNNN1) =text> TSNNN1

DefNNN = TNNNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNNNTWO1
DefNNN = '���' TNNNTWO1<c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=nom> '-' ['���'] Def1<c=nom> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=ins> '��' '��������' Def1<c=acc> =text> #TNNNTWO1
DefNNN = '���' TNNNTWO1<c=ins> '��' '��������' DefXXX1<c=acc> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = Def1 "," Pn1<�������> "��" ["�������"] "��������" TNNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=ins> "�����" "��������" Def1<c=acc>  =text> #TNNNTWO1
DefNNN = Def1<c=acc> ["��"] "�����" "��������" TNNNTWO1<c=ins>  =text> #TNNNTWO1
DefNNN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" TNNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #TNNNTWO1
DefNNN = Def1<c=acc> "�����" "��������" TNNNTWO1<c=ins>  =text> #TNNNTWO1
DefNNN = Def1<c=acc> ["������"] "�������" TNNNTWO1<c=ins> =text> #TNNNTWO1
DefNNN = "�������" TNNNTWO1<c=ins> Def1<c=acc>  =text> #TNNNTWO1
DefNNN = "�������" Def1<c=acc> TNNNTWO1<c=ins> =text> #TNNNTWO1
DefNNN = "�������" Def1<c=acc> TNNNTWO1<c=nom> =text> #TNNNTWO1
DefNNN = Def1<c=acc> "," Pn1<�������> "�������" TNNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNNNTWO1
DefNNN = Def1<c=acc> "�����" "�������" TNNNTWO1<c=ins> =text> #TNNNTWO1
DefNNN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] TNNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNNNTWO1
DefNNN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" TNNNTWO1<c=ins> =text> #TNNNTWO1
DefNNN = "�����" "����" Pa1<�������, f=short> TNNNTWO1<c=ins> <Pa1.n=TNNNTWO1.n> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=ins> ["\("MSPNNN1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #TNNNTWO1
DefNNN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> TNNNTWO1<c=ins> <Def1.n=V1.n> =text> #TNNNTWO1
DefNNN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> TNNNTWO1<c=ins> <V1.n=Def1.n> =text> TNNNTWO1
DefNNN = Def1<c=nom> "����������" ["�����" "���������" "-"] TNNNTWO1<c=nom> =text> #TNNNTWO1
DefNNN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> TNNNTWO1<c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = Def1<c=acc> "�������" "��������" TNNNTWO1<c=ins> =text> #TNNNTWO1
DefNNN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] TNNNTWO1<c=ins> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=ins> "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] TNNNTWO1<c=ins> <Def1=Pa1> =text> #TNNNTWO1
DefNNN = Pa1<��������> [Prep1<c=prep>]  N1<��������> TNNNTWO1<c=nom> <Pa1.n=N1.n> =text> #TNNNTWO1
DefNNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNNNTWO1<c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNNNTWO1
DefNNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" TNNNTWO1<c=nom> <Def1.n=V1.n> =text> #TNNNTWO1
DefNNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNNNTWO1<c=gen> <Def1.n=V1.n> =text> #TNNNTWO1
DefNNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" TNNNTWO1<c=nom> <Def1.n=V1.n> =text> #TNNNTWO1
DefNNN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" TNNNTWO1<c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #TNNNTWO1
DefNNN = "���" Pa1<��������> TNNNTWO1 <Pa1=TNNNTWO1> =text> #TNNNTWO1
DefNNN = {"�.�." | "�" "." "�" "."}<1,1> TNNNTWO1 =text> #TNNNTWO1
DefNNN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] TNNNTWO1<c=ins> <Pa1=Def1>  =text> #TNNNTWO1
DefNNN = Def1 "," Pa1<��������> TNNNTWO1<c=nom> <Pa1=Def1> =text> #TNNNTWO1
DefNNN = Def1 { "," | "\(" }<1,1> Pa1<�������> TNNNTWO1<c=ins> <Def1=Pa1> =text> #TNNNTWO1
DefNNN = Pn1 V1<��������, t=pres, p=3, m=ind> TNNNTWO1<c=ins> <Pn1=V1> =text> #TNNNTWO1
DefNNN = "���" TNNNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "���" TNNNTWO1<c=ins> "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "���" TNNNTWO1<c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "�����" "��������" "���" TNNNTWO1<c=ins> Def1<c=acc> =text> #TNNNTWO1
DefNNN = "������" "�������" TNNNTWO1<c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "������" "�������" TNNNTWO1<c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "���" "��������" TNNNTWO1<c=nom> "�����" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "���" TNNNTWO1<c=ins> "�������" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "���" TNNNTWO1<c=ins> "��" "��������" Def1<c=acc> =text> #TNNNTWO1
DefNNN = "���" TNNNTWO1<c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #TNNNTWO1
DefNNN = "���" "��������" TNNNTWO1<c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <TNNNTWO1.n=V1.n> =text> #TNNNTWO1
DefNNN = N1<������> TNNNTWO1<c=nom> =text> #TNNNTWO1
DefNNN = "���" "������" TNNNTWO1<c=nom> "����������" Def1<c=nom> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=nom> ["�"] "����" Def1<c=nom> =text> #TNNNTWO1
DefNNN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> TNNNTWO1<c=gen> =text> #TNNNTWO1
DefNNN = TNNNTWO1<c=nom> ["�"] "���" Def1<c=nom> =text> #TNNNTWO1
DefNNN = TNNNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <TNNNTWO1.c=Def1.c> =text> #TNNNTWO1
DefNNN = Pr1 TNNNTWO1 {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <TNNNTWO1.c=Def1.c> =text> #TNNNTWO1

NPMSPNNN = N1 N2<c=gen> N3<c=gen> (N1) =text> "N1[" #N1 "] N2[" #N2 ",c=gen] N3[" #N3 ",c=gen] (N1) =text] N1 N2[c=gen] N3[c=gen]"
NPTNNNTWOSyn = NPMSPNNN1 ["\("NPMSPNNN2"\)"] <NPMSPNNN1.c=NPMSPNNN2.c> (NPMSPNNN1) =text> NPMSPNNN1
NPTNNNTWO = NPTNNNTWOSyn1 [[","] "���" ["������"] NPTNNNTWOSyn2] <NPTNNNTWOSyn1.c=NPTNNNTWOSyn2.c> (NPTNNNTWOSyn1) =text> NPTNNNTWOSyn1

NPDefNNN = NPTNNNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNNNTWO1 
NPDefNNN = '���' NPTNNNTWO1 <c=ins> [Prep1<c=prep>] ['������' | '�����'] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=nom> '-' ['���'] Def1<c=nom> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=ins> '��' '��������' Defs1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = '���' NPTNNNTWO1 <c=ins> '��' '��������' DefXXX1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = Def1 "," Pn1<�������> "��" ["�������"] "��������" NPTNNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=ins> "�����" "��������" Def1<c=acc>  =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> ["��"] "�����" "��������" NPTNNNTWO1 <c=ins>  =text> #NPTNNNTWO1 
NPDefNNN = Def1 "," Pn1<�������> ["�����"] "�����" ["�������"] "��������" NPTNNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a>  =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> "�����" "��������" NPTNNNTWO1 <c=ins>  =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> ["������"] "�������" NPTNNNTWO1 <c=ins> =text> #NPTNNNTWO1 
NPDefNNN = "�������" NPTNNNTWO1 <c=ins> Def1<c=acc>  =text> #NPTNNNTWO1 
NPDefNNN = "�������" Def1<c=acc> NPTNNNTWO1 <c=ins> =text> #NPTNNNTWO1 
NPDefNNN = "�������" Def1<c=acc> NPTNNNTWO1 <c=nom> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> "," Pn1<�������> "�������" NPTNNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> "�����" "�������" NPTNNNTWO1 <c=ins> =text> #NPTNNNTWO1 
NPDefNNN = Def1 "," Pn1<�������> "�����" ["����" "��"] "�������" ["�����"] NPTNNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> {"������"|"�������"}<1,1> "�������" NPTNNNTWO1 <c=ins> =text> #NPTNNNTWO1 
NPDefNNN = "�����" "����" Pa1<�������, f=short> NPTNNNTWO1 <c=ins> <Pa1.n=NPTNNNTWO1 .n> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=ins> ["\("MSP1<c=gen>"\)" | "\("Prep1<c=prep>"\)"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <V1.n=Def1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> ["�������" | "," "���" "�������" "," | "������" | "������" | "�"] V1<����������, t=pres, p=3, m=ind> NPTNNNTWO1 <c=ins> <Def1.n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> Prep1<c=prep> V1<����������, t=pres, p=3, m=ind> NPTNNNTWO1 <c=ins> <V1.n=Def1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> "����������" ["�����" "���������" "-"] NPTNNNTWO1 <c=nom> =text> #NPTNNNTWO1 
NPDefNNN = Def1"," Pn1<�������> ["�����������"] V1<����������, t=pres, p=3, m=ind> NPTNNNTWO1 <c=ins> <Def1.g=Pn1.g, Def1.n=Pn1.n, Def1.a=Pn1.a> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> "�������" "��������" NPTNNNTWO1 <c=ins> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> [Prep1<c=prep>]  "��������" ["�����"] NPTNNNTWO1 <c=ins> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> Pa1<�������, f=short> [Prep1<c=prep>] NPTNNNTWO1 <c=ins> <Def1=Pa1> =text> #NPTNNNTWO1 
NPDefNNN = Pa1<��������> [Prep1<c=prep>]  N1<��������> NPTNNNTWO1 <c=nom> <Pa1.n=N1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNNNTWO1 <c=gen> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> V1<��������, t=past, p=3, m=ind> "��������" NPTNNNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNNNTWO1 <c=gen> <Def1.n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=nom> V1<������, t=pres, p=3, m=ind> "��������" NPTNNNTWO1 <c=nom> <Def1.n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = Def1<c=acc> "," Pn1<�������> ["������������"] V1<��������, t=past, p=3, m=ind> "��������" NPTNNNTWO1 <c=nom> <Def1.n=V1.n, Def1.g=V1.g> =text> #NPTNNNTWO1 
NPDefNNN = "���" Pa1<��������> NPTNNNTWO1  <Pa1=NPTNNNTWO1 > =text> #NPTNNNTWO1 
NPDefNNN = {"�.�." | "�" "." "�" "."}<1,1> NPTNNNTWO1  =text> #NPTNNNTWO1 
NPDefNNN = Def1 {","|"\("}<1,1> Pa1<��������> ["�����"] NPTNNNTWO1 <c=ins> <Pa1=Def1>  =text> #NPTNNNTWO1 
NPDefNNN = Def1 "," Pa1<��������> NPTNNNTWO1 <c=nom> <Pa1=Def1> =text> #NPTNNNTWO1 
NPDefNNN = Def1 { "," | "\(" }<1,1> Pa1<�������> NPTNNNTWO1 <c=ins> <Def1=Pa1> =text> #NPTNNNTWO1 
NPDefNNN = Pn1 V1<��������, t=pres, p=3, m=ind> NPTNNNTWO1 <c=ins> <Pn1=V1> =text> #NPTNNNTWO1 
NPDefNNN = "���" NPTNNNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "���" NPTNNNTWO1 <c=ins> "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "���" NPTNNNTWO1 <c=ins> ["�����" | "�" "�����" "������" | "��"] "�����" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "�����" "��������" "���" NPTNNNTWO1 <c=ins> Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "������" "�������" NPTNNNTWO1 <c=acc> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "������" "�������" NPTNNNTWO1 <c=nom> "," "���" "�������" "�����" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "���" "��������" NPTNNNTWO1 <c=nom> "�����" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "���" NPTNNNTWO1 <c=ins> "�������" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "���" NPTNNNTWO1 <c=ins> "��" "��������" Def1<c=acc> =text> #NPTNNNTWO1 
NPDefNNN = "���" NPTNNNTWO1 <c=ins> [Prep1<c=prep>] ["������" | "�����"] V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <Def1.n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = "���" "��������" NPTNNNTWO1 <c=nom> V1<����������, t=pres, p=3, m=ind> Def1<c=nom> <NPTNNNTWO1 .n=V1.n> =text> #NPTNNNTWO1 
NPDefNNN = N1<������> NPTNNNTWO1 <c=nom> =text> #NPTNNNTWO1 
NPDefNNN = "���" "������" NPTNNNTWO1 <c=nom> "����������" Def1<c=nom> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=nom> ["�"] "����" Def1<c=nom> =text> #NPTNNNTWO1 
NPDefNNN = ["�������������" | "�����"] "������" N1<�������, c=acc, n=sing> NPTNNNTWO1 <c=gen> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1 <c=nom> ["�"] "���" Def1<c=nom> =text> #NPTNNNTWO1 
NPDefNNN = NPTNNNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Def1 <NPTNNNTWO1 .c=Def1.c> =text> #NPTNNNTWO1 
NPDefNNN = Pr1 NPTNNNTWO1  {"\(" | "," }<1,1> {"�.�."|"��" "����"|"�." "�."}<1,1> Pr1 Def1 <NPTNNNTWO1 .c=Def1.c> =text> #NPTNNNTWO1