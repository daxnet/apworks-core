# Test Driven Development \(TDD\)

**1\) Introduction to Testing**

> What is TDD?
>
> * Development Methodology
>   * Test First
>   * Developing code with testing in mind
> * Development Process or Testing Process ?
> * Who should tests software?
> * Testable Software Design
> * AnAgile Approach
>   * Kiss, Yangi,Dry, FateIt\(TillYouMakeld\)

**2\) TDD Methodology**

**3\) OOP Quick Review**

**4\) Dependency Injection \(DI\)**

**5\) TDD Real World Requirements**

**6\) Test Doubles \(Mocks, Stubs\)**

**7\) TDD and Refactoring**

**8\) Test Code Quality**

**9\) Unit Testing Tools**

**10\) Behavioral Driven Development \(BDD\)**

**11\) Continious Delivery \(CD\)**

{% hint style="info" %}
 Super-powers are granted randomly so please submit an issue if you're not happy with yours.
{% endhint %}



**What is TDD?**

> * Development Methodology
>   * Test First
>   * Developing code with testing in mind
> * Development Process or Testing Process ?
> * Who should tests software?
> * Testable Software Design
> * AnAgile Approach
>   * Kiss, Yangi,Dry, FateIt\(TillYouMakeld\)
>
> ![](https://skywalkerod.gitbooks.io/test-driven-development-tdd/content/assets/import_TestD.png)

* **Stress/ Load Testing**

  > Birden fazla kullanıcının aynı anda işlem yaptığı zaman yazılan code nasıl performan sağladığı ve herhangi bir hata oluyor mu bunu öğrenebileceğimiz test methodolijisi.

* **Platform Testing**

  > Uygulamanın durduğu platform configlerde bir değişikliği görmek için uygulanan bir test yöntemi.

* **Integration Testing**

  > **Ext : UI Testing**
  >
  > Thirt -Part uygulamalara ve API bağlı code yazıldığında bunun uygunluğunu test etmeyi sağlar.

* **Regression Testing**
  * **Unit Testing**
* **Static vs Dynamic Testing**

![](https://skywalkerod.gitbooks.io/test-driven-development-tdd/content/assets/import_1.png)

* **BlackBox Testing**

  > İçeriği dikkate alınmadan sadece input ve output bakılarak yazılan test methodolijisi.

* **WhiteBox Testing**

  > Genelde yazan kişinin, yazılan code dikkate alınarak tüm fonksiyonelitiyi dikkate alarak yazdığı testtir.

* **Manual vs Automatic Testing**

#### [Unit test](https://www.slideshare.net/KeytorcSoftwareTesti/50-soruda-yazlm-testi/28) {#unit-test}

## F.I.R.S.T Principles of Unit Testing [....](https://github.com/ghsukumar/SFDC_Best_Practices/wiki/F.I.R.S.T-Principles-of-Unit-Testing#fast-isolatedindependent-repeatable-self-validating-and-thoroughtimely) {#first-principles-of-unit-testing--}

[**Eğitim Videosu**](https://www.udemy.com/unit-test-yazma-sanati/learn/v4/overview)

![](https://skywalkerod.gitbooks.io/test-driven-development-tdd/content/assets/import_green.png)

Her Unit Test hızlı olmalıdır. Fast

Unit Test yazılması için ortamlardan ayrı bağımsız olmalı. Isolated

```text
  [TestClass]
    public class TDDProgressTest
    {
        [TestMethod]
        public void Firstmethod()
        {
            int a = 9;
            int b = 5;

            int c = a + b;

            a = b;

            Assert.AreSame(c, a + b, "Same");
            Assert.AreNotSame(c, a + b, "Not Same");
        }
    }
```



## **OOP Quick Review** {#oop-quick-review}

* **Inheritance**
* **Polymorphism**
* **Abstraction**
* **Composition**
* **Encapsulation**
* **Depend upon abstractions**
* **Composition over inheritance**
* **S.O.L.I.D**

