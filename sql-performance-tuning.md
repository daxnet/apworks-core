# SQL Performance Tuning



1\) sys.dm\__os\_per\_formance_\_counters\_

```text
     Bu genelde bir sonraki sürümde kullanılmayacak item'ları ve ne kadar kullanılıldığı gösterir.
```

![](https://skywalkerod.gitbooks.io/sql-performance-tuning/content/assets/imporDB.png)

![](https://skywalkerod.gitbooks.io/sql-performance-tuning/content/assets/import_Root.png)

* > Update ve Delete işlemşerinde log datası tutulur. Data boyutum aynı olsada log datam devamlı artar bunu temizlemenin iki yöntemi mevcut.
  >
  > * Shink yöntemi \(Bu fazla önerilmez\)
  > * Log Backup yöntemi.
* GAM Page
* Row'lar page oluşur her page 8 kb tır, burada genelde 96 btye table name ve tipler enel şeyler oluşturulur.
  * diğer kalan kısımda datanın kendisi tutar.

#### Buffer pool

> Buffer \(tampon saha\), verilerin I/O işlemlerinden sonra belleğe yazılmadan önce uğradıkları bir sahadır. Bufferlar I/O işlemi sırasında kullanıcının beklemesini engellemek için kullanılırlar. Bellekten okumak ve belleğe yazmak maliyetli bir işlemdir. Sistemi yorar ve hız olarak yavaştır. I/O aygıtlarından gelen veriler bu sebeple önce bir havuzda toplanır. Böylece bu havuz belirli miktarlarda dolduktan sonra toplu olarak belleğe yazılır. Bu sisteme performans kazandıran bir harekettir.

#### **Logicalread**

> SQL çalıştığı esnada, buffer cache\(Main memory\) üzerinde bir block’a erişme durumuna logical read denir. SQL çalıştığı esnada buffer cache’den okunan blok sayısıda o sorgu için yapılan logical read sayısını gösterir. Logical Read sayısı bize, çalıştırdığımız sorgumuzun kullandığı veri erişim yönteminin etkiliğini söyleyebilir çünkü;
>
> **–** Lojik okuma sayısı CPU utilizationımız hakkında bize bilgi verir. Çünkü Lojik okuma CPU’yu meşgul eden bir işlemdir. Lojik okuma sayısının çok olmasından CPU utilizationımızında fazla olduğu genellemesine varabiliriz.
>
> **–** Lojik okuma, fiziksel disk okumalarına sebebiyet vereceğinden dolayı I/O sayımız hakkındada bize bilgi vermektedir. Lojik okuma sayımızın fazla olması, fiziksel okuma sayımızında artacağına işaret eder, buda fazladan I/O yapmak anlamına gelir. I/O’nun performansımızı negatif anlamda en çok etkileyen faktör olduğunu düşünürsek bu bağlamda veri erişim yönteminin etkinliği hakkında çok kritik bir bilgiyide bize sunmaktadır.

* sys.dm\_os\_buffer\_descriptors
* `select * from sqlservers_start_time fromsys.dm_os_sys_info`
* CheckPoint, yapılan işlemleri mdf önce log db yazar, sorasında log datasını mdf taşır bu aşamada herhangi bir işlem yapıldığıms slq servers. işlemi geri almak için bunu kullanır.

> Recovery\__model_\_decs // Burada Recovery model aktif edildiğinde sql servers her zaman db Backup alır ve geri yükleme alanları oluşturur.

* Full -&gt; Veri kaybetme olasığı yüzde 1
  * Genelde logları backup almak gerekiyor Backup alınmadığında log DB şişer.
  * `backup log DBName to dick = 'nil'`
  * `select * from sysfn_dblognull,null) Bu komut Log dosyasını okumaya yarar.`
* Bulklog -&gt; yüzde 20 oranında veri kaybedilir
* Simple \(Eğer datalar önemli değilse bu kullanılır checkpoint çalıştığı anda log tablosunu boşaltır.\)

`select * from sys.dm_`_\`os_`_`sys\_info\`

> max\_workers\_count // Burada aynı anda kaç kullanıcı çalışır onu gönsterir.
>
> Bunu heplaması, cpu ve lisans bağlıdır ilk kurulum aşamasında hesaplar.

Admin User bağlandığı zaman prorgrees durdurabiliyoruz.

```sql
select *
from sys.configurations
where name like '%admin%'
go
so_configure 'remote admin connections',1
go
reconfigarios
```

> Burada bir connection her zamana acil durumlar için bırakmak için kullanılır. Eğer SQL servers işlme yapmıyorsa admin yetkisi olan bir kişiye bir bağlanır..

```sql
Select
from sys.dm_os_waiting_tasks
where session_id=gg
```

> Burada bekleyen progressleri görürsünüz...
>
> with ties -&gt; top 1 desen dahi onunla ilgili kaç sonuç varsa onu getirir.

```sql
Select  top 1  with ties, productID, Name , color, listprice
from Production.Product
order by listprice desc
```

