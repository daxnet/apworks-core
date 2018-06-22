# F.I.R.S.T Principles of Unit Testing



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

