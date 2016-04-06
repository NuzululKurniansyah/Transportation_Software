
load matrikskotadepot
clc
%load cut
%label4d = uicontrol('parent',winrpl,'style','text','string','Process','position',[350 20 100 20]);

x0= get(checkbox0,'value');
x1 = get(checkbox1,'value');
x2 = get(checkbox2,'value');
x3 = get(checkbox3,'value');
x4 = get(checkbox4,'value');
x5 = get(checkbox5,'value');
x6 = get(checkbox6,'value');
x7 = get(checkbox7,'value');
x8 = get(checkbox8,'value');
x9 = get(checkbox9,'value');
y = [ x0 x1 x2 x3 x4 x5 x6 x7 x8 x9 ];
posisi = find(y==0);
posisi1 = find(y==1);
[r,c]= size(posisi);
    %for n = 1:c
    matrikskotadepot(posisi(1,:),:)=[];
    %end
    matrikskotadepot(:,posisi(1,:))=[];
    %end
matriksjarakkota = matrikskotadepot;
save matriksjarakkota
if c>=8
    errordlg('Harus lebih dari 2 Kota','Program Error');
    dbstop if error
else
%--------------------------------------------------------
%Fungsi nnh= puzzle
%--------------------------------------------------------
n=1;
[sa,si]=size(matriksjarakkota);
%si=17;

%Step-1: Dilakukan perhitungan pergerakan pertama

A=matriksjarakkota(n,:);
x=min(A(A~=0));
posisi=find(A==x);
 
%kolom 1 disimpan dulu
start=matriksjarakkota(:,1);
 
%backup untuk data matrik asli
initial=matriksjarakkota;
    
 %for m=1:1:jum
for m=1:si;
    initial(m,posisi)=0;
    initial(m,n)=0;
end
    save initial

% looping dilakukan sebanyak h kali dengan rumus (h=si-2)
% 2 adalah proses awal dan proses akhir

h=si-2;
n=posisi;

%Step 2: Dilakukan proses looping setelah data matriks initial di save
    
for lm=1:h    
    A=initial(n,:);
    x1=min(A(A~=0));
    posisi1=find(A==x1);
    
    %show the movement looping
    jum(lm+0)=x1  ;  
    all_posisi(lm+0)=(posisi1);
    
    for m=1:si;
        initial(m,posisi1)=0;
    end
    
    n=posisi1;
    save initial
end

% Step 3: Hitung nilai pada kolom 1 dengan baris pisisi1 akhir

first=all_posisi(1,h);

% Step 4: Merubah nilai matrik kolom menjadi matrik baris
posisiku=all_posisi';
jumku=jum';

% Step 5: Menjumlahkan nilai hasil looping Step 2
y=sum(jum);

% Step 6: Mengatur kembali posisi awal sampai kahir
langkah=[posisi;posisiku;1]
jarak=[x;jumku;(start(posisi1,1))]

% Step 7: Menghitung nilai hasil Step 1+ Step 3 + Step 6
total=sum(jarak)
end
%-----------------------------------------------

%while jarak_minim > 180

%------------------------------------------------------
% Fungsi Cuckoo Search Algorithm With Levy Flight
%------------------------------------------------------

t = cputime;
maxgeneration = 10000;
%help cuckoountitled.m
disp('Computing ... it may take a few minutes.');
load ('matriksjarakkota.mat');
%load ('kota.mat');
%load ('akhir.mat');
%load ('matriksjarak.mat');
[jin,jun]=size(matriksjarakkota);
mobil2= matriksjarakkota((2:jin),(2:jun));
kota = matriksjarakkota(1,(2:jun))';
akhir = matriksjarakkota((2:jin),1);
[r,c] = size(mobil2);
nk=c; %jumlah kota
np=20;
pa=0.25;
nest = rand(np,nk);
%pengurutan nilai random secara ascending untuk mendapatkan rute
[min1, perm]=sort(nest,2);
%perm_tsp=[perm perm(:,1)]; %agar balik ke kota awal
%Evaluasi nilai fungsi tujuan dan penghitungan intensity
jarak=zeros(np,1);
%Perhitungan Jarak
for i=1:1:np
    x1=perm(i,:);
    jarak(i)=jartsp(x1,mobil2,kota,akhir);
    %akses dari fungsi jartsp.m
end
f=jarak;
tbest = nest;
minftot=[];
%------------------------------Iterasi------------------------------------%
for k=1:maxgeneration,     %%%%% start iterations
 
%Sorting Fitness
[Lightn,Index]=sort(jarak);
%Ranking the Nest
nso=nest; 
ns_tmp=nest;
for i=1:np,
 nest(i,:)=ns_tmp(Index(i),:);
end
%Find the current best
nbest=nest(1,:);
Lightbest=Lightn(1);
% For output only
fbest=Lightbest;
%-----------------Move all Cuckoo towards the best one-------------------%
% Updating Cuckoo
np = size(nest,1);
beta=3/2;
sigma=(1.33*sin(pi*beta/2)/(0.9*beta*2^((beta-1)/2)))^(1/beta);
for j=1:np,
s=nest(j,:);
u=randn(size(s))*sigma;
v=randn(size(s));
step=u./abs(v).^(1/beta);
stepsize=0.01*step.*(s-Lightbest);
s=s+stepsize.*randn(size(s));
np=size(nest,1);
K=rand(size(nest))>pa;
nestn1=nest(randperm(np),:);
nestn2=nest(randperm(np),:);
%% New solution by biased/selective random walks
stepsize=rand*(nestn1-nestn2);
new_nest(j,:)=nest(j,:)+stepsize(j,:).*K(j,:);
end
% end for j
%Sorting Cuckoo yang baru dan temukan yang paling baik untuk sekarang
[min1, perm]=sort(new_nest,2);
%perm_tsp1=[perm perm(:,1)];
%evaluasi lagi nilai permutasi TSP yang baru lagi
jarak = zeros(np,1);
for i = 1:np
    x1=perm(i,:);
    jarak(i)=jartsp(x1,mobil2,kota,akhir);
end
f = jarak;
changerow = f < fbest;
fbest = fbest.*(1-changerow)+f.*changerow;
tbest(find(changerow),:)=nest(find(changerow),:);
[minf,idk]=min(fbest);
minftot=[minftot;minf];
end
%outputsolution
lastone = tbest;
[min1,perm]=sort(lastone,2);
%perm_tsp=[perm perm(:,1)];
%Evaluasi paling akhir banget
jarak=zeros(np,1);
for i = 1:np
    x1=perm(i,:);
    jarak(i)=jartsp(x1,mobil2,kota,akhir);
end
f=jarak;
[jarak_minim,idx]=min(f);
jarak_minim
rute_optim=perm(idx,:);
awal=rute_optim(1,1);
akhiri=rute_optim(1,nk);
rute_akhir=[0 rute_optim 0]
t = cputime - t

%----------------------------------------------------------
%Fungsi call back
%-----------------------------------------------------------
if jarak_minim <= total
    jarak_minim=jarak_minim
else
while jarak_minim>=total   
t = cputime;
maxgeneration = 10000;
%help cuckoountitled.m
disp('Computing ... it may take a few minutes.');
load ('matriksjarakkota.mat');
%load ('kota.mat');
%load ('akhir.mat');
%load ('matriksjarak.mat');
[jin,jun]=size(matriksjarakkota);
mobil2= matriksjarakkota((2:jin),(2:jun));
kota = matriksjarakkota(1,(2:jun))';
akhir = matriksjarakkota((2:jin),1);
[r,c] = size(mobil2);
nk=c; %jumlah kota
np=20;
pa=0.25;
nest = rand(np,nk);
%pengurutan nilai random secara ascending untuk mendapatkan rute
[min1, perm]=sort(nest,2);
%perm_tsp=[perm perm(:,1)]; %agar balik ke kota awal
%Evaluasi nilai fungsi tujuan dan penghitungan intensity
jarak=zeros(np,1);
%Perhitungan Jarak
for i=1:1:np
    x1=perm(i,:);
    jarak(i)=jartsp(x1,mobil2,kota,akhir);
    %akses dari fungsi jartsp.m
end
f=jarak;
tbest = nest;
minftot=[];
%------------------------------Iterasi------------------------------------%
for k=1:maxgeneration,     %%%%% start iterations
 
%Sorting Fitness
[Lightn,Index]=sort(jarak);
%Ranking the Nest
nso=nest; 
ns_tmp=nest;
for i=1:np,
 nest(i,:)=ns_tmp(Index(i),:);
end
%Find the current best
nbest=nest(1,:);
Lightbest=Lightn(1);
% For output only
fbest=Lightbest;
%-----------------Move all Cuckoo towards the best one-------------------%
% Updating Cuckoo
np = size(nest,1);
beta=3/2;
sigma=(1.33*sin(pi*beta/2)/(0.9*beta*2^((beta-1)/2)))^(1/beta);
for j=1:np,
s=nest(j,:);
u=randn(size(s))*sigma;
v=randn(size(s));
step=u./abs(v).^(1/beta);
stepsize=0.01*step.*(s-Lightbest);
s=s+stepsize.*randn(size(s));
np=size(nest,1);
K=rand(size(nest))>pa;
nestn1=nest(randperm(np),:);
nestn2=nest(randperm(np),:);
%% New solution by biased/selective random walks
stepsize=rand*(nestn1-nestn2);
new_nest(j,:)=nest(j,:)+stepsize(j,:).*K(j,:);

end % end for j
%Sorting Cuckoo yang baru dan temukan yang paling baik untuk sekarang
[min1, perm]=sort(new_nest,2);
%perm_tsp1=[perm perm(:,1)];
%evaluasi lagi nilai permutasi TSP yang baru lagi
jarak = zeros(np,1);
for i = 1:np
    x1=perm(i,:);
    jarak(i)=jartsp(x1,mobil2,kota,akhir);
end
f = jarak;
changerow = f < fbest;
fbest = fbest.*(1-changerow)+f.*changerow;
tbest(changerow,:)=nest(changerow,:);
[minf,idk]=min(fbest);
minftot=[minftot;minf];
end
%outputsolution
lastone = tbest;
[min1,perm]=sort(lastone,2);
%perm_tsp=[perm perm(:,1)];
%Evaluasi paling akhir banget
jarak=zeros(np,1);
for i = 1:np
    x1=perm(i,:);
    jarak(i)=jartsp(x1,mobil2,kota,akhir);
end
f=jarak;
[jarak_minim,idx]=min(f);
jarak_minim;
rute_optim=perm(idx,:);
awal=rute_optim(1,1);
akhiri=rute_optim(1,nk);
rute_akhir=[0 rute_optim 0];
t = cputime - t;
end
end
yel=num2str(rute_akhir);
set(Edittext1,'string',yel);
ytext=rute_akhir;
[rr,cc]= size(ytext);
%for xx=1:cc
  %end
set(Edittext2,'string',jarak_minim);
%Edittext1 = uicontrol('parent',win3,'style','text','string',yel,'position',[75 50 100 20]); 
