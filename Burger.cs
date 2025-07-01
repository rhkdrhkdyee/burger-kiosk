using System.Windows.Forms;
using System.Drawing;
using System.Text;
namespace c__project
{
    public partial class Burger : Form
    {

        Dictionary<string, int> ingredientCount = new Dictionary<string, int>();



        List<string> GetFormattedIngredientSummary()
        {
            List<string> lines = new List<string>();

            var 재료 = ingredientCount.Where(i => !IsDrink(i.Key) && !IsSide(i.Key));
            var 음료 = ingredientCount.Where(i => IsDrink(i.Key));
            var 사이드 = ingredientCount.Where(i => IsSide(i.Key));

            if (재료.Any())
            {
                lines.Add("[재료]");
                foreach (var item in 재료)
                {
                    // 해당 품목의 가격 가져오기
                    int price = selectedIngredients.FirstOrDefault(i => i.Name == item.Key)?.Price ?? 0;
                    int totalPrice = price * item.Value;  // 품목의 수량 * 가격
                    lines.Add($"{item.Key}: {item.Value}개  {totalPrice:N0}원");  // 가격 추가
                }
            }

            if (사이드.Any())
            {
                lines.Add("");
                lines.Add("[사이드]");
                foreach (var item in 사이드)
                {
                    // 해당 품목의 가격 가져오기
                    int price = selectedIngredients.FirstOrDefault(i => i.Name == item.Key)?.Price ?? 0;
                    int totalPrice = price * item.Value;  // 품목의 수량 * 가격
                    lines.Add($"{item.Key}: {item.Value}개  {totalPrice:N0}원");  // 가격 추가
                }
            }

            if (음료.Any())
            {
                lines.Add("");
                lines.Add("[음료]");
                foreach (var item in 음료)
                {
                    // 해당 품목의 가격 가져오기
                    int price = selectedIngredients.FirstOrDefault(i => i.Name == item.Key)?.Price ?? 0;
                    int totalPrice = price * item.Value;  // 품목의 수량 * 가격
                    lines.Add($"{item.Key}: {item.Value}개  {totalPrice:N0}원");  // 가격 추가
                }
            }

            return lines;
        }

        HashSet<string> drinkNames = new HashSet<string>
        {
            "코카콜라", "스프라이트", "오렌지주스", "팹시제로", "아메리카노", "탄산수",
            "닥터페퍼", "삼다수"
        };
        bool IsDrink(string name)
        {
            return drinkNames.Contains(name);
        }
        PictureBox topBun;
        PictureBox bottomBun;


        HashSet<string> sideNames = new HashSet<string>
        {
            "감자튀김", "치즈스틱", "치킨너겟", "웨지감자", "샐러드", "어니언링","특제소스"
        };
        bool IsSide(string name)
        {
            return sideNames.Contains(name);
        }

        private void InitializeBuns()
        {
            topBun = new PictureBox();
            bottomBun = new PictureBox();

            // top 번 설정
            topBun.Width = pl_Burger.Width;
            topBun.Height = 50;
            topBun.Top = 0;
            topBun.Left = 0;
            topBun.SizeMode = PictureBoxSizeMode.StretchImage;
            pl_Burger.Controls.Add(topBun);

            // bottom 번 설정
            bottomBun.Width = pl_Burger.Width;
            bottomBun.Height = 50;
            bottomBun.Left = 0;
            bottomBun.Top = pl_Burger.Height - 40;
            bottomBun.SizeMode = PictureBoxSizeMode.StretchImage;
            pl_Burger.Controls.Add(bottomBun);

        }




        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBuns();
            lb_Total.Text = $"Total Price : 0원";

            LoadAdImages();
            pb_AD.Image = adBannerImages[0];
            pb_AD.SizeMode = PictureBoxSizeMode.StretchImage;
            timer1.Start();

            LoadGifAd(); // gif 표시
        }


        void SelectBun(string bunType)
        {
            if (bunType == "일반번")
            {
                topBun.Image = Properties.Resource.빵1;
                bottomBun.Image = Properties.Resource.빵2;

                AddOrReplaceBun("일반번", 2000);
            }
            else if (bunType == "버터번")
            {
                topBun.Image = Properties.Resource.빵1;
                bottomBun.Image = Properties.Resource.빵2;

                AddOrReplaceBun("버터번", 2500);
            }
        }
        string currentBunName = "";
        void AddOrReplaceBun(string name, int price)
        {
            // 같은 번이면 아무 것도 안 함
            if (currentBunName == name)
                return;

            // 기존 번 제거
            if (!string.IsNullOrEmpty(currentBunName))
            {
                var oldBun = selectedIngredients.FirstOrDefault(i => i.Name == currentBunName);
                if (oldBun != null)
                {
                    selectedIngredients.Remove(oldBun);
                    totalPrice -= oldBun.Price;

                    if (ingredientCount.ContainsKey(currentBunName))
                        ingredientCount.Remove(currentBunName);
                }
            }

            // 새 번 추가
            selectedIngredients.Insert(0, new Ingredient
            {
                Name = name,
                Price = price,
                Picture = null
            });

            totalPrice += price; //  가격 추가

            ingredientCount[name] = 1;
            currentBunName = name;

            UpdatePriceLabel();
            UpdateSeparateIngredientLists();
        }


        // 재료 정보를 저장할 클래스
        class Ingredient
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public PictureBox Picture { get; set; }
        }

        // 현재 선택된 재료들을 담을 리스트
        List<Ingredient> selectedIngredients = new List<Ingredient>();

        // 총 가격
        int totalPrice = 0;

        int ingredientHeight = 50; // 재료 하나의 높이 

        void AddIngredient(string name, int price, Image image)
        {
            PictureBox pb = new PictureBox();
            pb.Image = image;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Width = pl_Burger.Width;
            pb.Height = ingredientHeight;
            pb.Left = 0;

            // Y 위치는 맨 아래에서부터 쌓기
            int count = selectedIngredients.Count;
            pb.Top = pl_Burger.Height - ((count + 1) * ingredientHeight);

            pb.Click += (s, e) => RemoveIngredient(pb);

            Ingredient ing = new Ingredient
            {
                Name = name,
                Price = price,
                Picture = pb
            };

            selectedIngredients.Add(ing);
            pl_Burger.Controls.Add(pb);

            totalPrice += price;
            UpdatePriceLabel();
            if (ingredientCount.ContainsKey(name))
                ingredientCount[name]++;
            else
                ingredientCount[name] = 1;
            UpdateSeparateIngredientLists();
        }

        void RemoveIngredient(PictureBox pb)
        {
            var ing = selectedIngredients.FirstOrDefault(i => i.Picture == pb);

            if (ing == null)
                return; // null이면 바로 종료 

            selectedIngredients.Remove(ing);
            pl_Burger.Controls.Remove(pb);
            totalPrice -= ing.Price;

            // 위치 재정렬
            for (int i = 0; i < selectedIngredients.Count; i++)
            {
                var p = selectedIngredients[i].Picture;
                if (p != null)
                    p.Top = pl_Burger.Height - ((i + 1) * ingredientHeight);
            }

            UpdatePriceLabel();

            if (ingredientCount.ContainsKey(ing.Name))
            {
                ingredientCount[ing.Name]--;
                if (ingredientCount[ing.Name] == 0)
                    ingredientCount.Remove(ing.Name);
            }


            UpdateSeparateIngredientLists();
        }


        void UpdatePriceLabel()
        {
            int totalCount = ingredientCount.Values.Sum(); // 정확한 개수
            lb_Total.Text = $"Total: {totalPrice}원";
        }


        public Burger()
        {
            InitializeComponent();
            lstDrink.SelectedIndexChanged += lstDrink_SelectedIndexChanged;
            lstSide.SelectedIndexChanged += lstSide_SelectedIndexChanged;
        }
        int takeout = 0;
        private void bt_In_Click(object sender, EventArgs e)
        {
            pl_Menu.Visible = false;
            pl_OrderBurger.Visible = true;
            plBurgerButton.Visible = true;
        }

        private void bt_Out_Click(object sender, EventArgs e)
        {
            pl_Menu.Visible = false;
            pl_OrderBurger.Visible = true;
            plBurgerButton.Visible = true;
            takeout = 1;
        }

        private void bt_Bun_Click(object sender, EventArgs e)
        {
            pl_Ingredient.Visible = false;
            pl_Bun.Visible = true;
            BackIngre = 1;
        }

        private void bt_Patty_Click(object sender, EventArgs e)
        {
            if (otheringre == 0)
            {
                MessageBox.Show("번을 먼저 선택하세요");
            }
            else
            {
                pl_Ingredient.Visible = false;
                pl_Patty.Visible = true;
                BackIngre = 2;
            }
        }

        private void bt_Vegetable_Click(object sender, EventArgs e)
        {
            if (otheringre == 0)
            {
                MessageBox.Show("번을 먼저 선택하세요");
            }
            else
            {
                pl_Ingredient.Visible = false;
                pl_Vegetable.Visible = true;
                BackIngre = 3;
            }
        }

        private void bt_Sauce_Click(object sender, EventArgs e)
        {
            if (otheringre == 0)
            {
                MessageBox.Show("번을 먼저 선택하세요");
            }
            else
            {
                pl_Ingredient.Visible = false;
                pl_Sauce.Visible = true;
                BackIngre = 4;
            }
        }

        private void bt_Etc_Click(object sender, EventArgs e)
        {
            if (otheringre == 0)
            {
                MessageBox.Show("번을 먼저 선택하세요");
            }
            else
            {
                pl_Ingredient.Visible = false;
                pl_Etc.Visible = true;
                BackIngre = 5;
            }
        }
        int otheringre = 0;
        private void bt_BunNormal_Click(object sender, EventArgs e)
        {
            SelectBun("일반번");
            otheringre = 1;
        }

        private void bt_BunButter_Click(object sender, EventArgs e)
        {
            SelectBun("버터번");
            otheringre = 1;
        }

        private void bt_CowM_Click(object sender, EventArgs e)
        {
            AddIngredient("소고기패티(미디엄)", 2500, Properties.Resource.패티);
        }

        private void bt_CowW_Click(object sender, EventArgs e)
        {
            AddIngredient("소고기패티(웰던)", 2500, Properties.Resource.패티);
        }

        private void bt_Bacon_Click(object sender, EventArgs e)
        {
            AddIngredient("베이컨", 2000, Properties.Resource.베이컨);
        }

        private void bt_Sangchi_Click(object sender, EventArgs e)
        {
            AddIngredient("양상추", 1000, Properties.Resource.양상추);
        }

        private void bt_Tomato_Click(object sender, EventArgs e)
        {
            AddIngredient("토마토", 1000, Properties.Resource.토마토);
        }

        private void bt_Onion_Click(object sender, EventArgs e)
        {
            AddIngredient("양파", 1000, Properties.Resource.양파);
        }

        private void bt_Pickle_Click(object sender, EventArgs e)
        {
            AddIngredient("피클", 1000, Properties.Resource.피클);
        }

        private void bt_SauceHot_Click(object sender, EventArgs e)
        {
            AddIngredient("핫소스", 500, Properties.Resource.소스);
        }

        private void bt_SauceB_Click(object sender, EventArgs e)
        {
            AddIngredient("버거소스", 500, Properties.Resource.소스);
        }

        private void bt_Cheese_Click(object sender, EventArgs e)
        {
            AddIngredient("치즈", 1000, Properties.Resource.치즈);
        }

        private void bt_Egg_Click(object sender, EventArgs e)
        {
            AddIngredient("계란", 1000, Properties.Resource.계란);
        }

        int BackIngre = 0;

        private void bt_BackIngre_Click(object sender, EventArgs e)
        {
            if (BackIngre == 1)
            {
                pl_Bun.Visible = false;
                pl_Ingredient.Visible = true;
                BackIngre = 0;
            }
            else if (BackIngre == 2)
            {
                pl_Patty.Visible = false;
                pl_Ingredient.Visible = true;
                BackIngre = 0;
            }
            else if (BackIngre == 3)
            {
                pl_Vegetable.Visible = false;
                pl_Ingredient.Visible = true;
                BackIngre = 0;
            }
            else if (BackIngre == 4)
            {
                pl_Sauce.Visible = false;
                pl_Ingredient.Visible = true;
                BackIngre = 0;
            }
            else if (BackIngre == 5)
            {
                pl_Etc.Visible = false;
                pl_Ingredient.Visible = true;
                BackIngre = 0;
            }
        }

        void AddDrink(string name, int price)
        {
            // 재료와 동일하게 중복 가능하니까 그냥 추가
            selectedIngredients.Add(new Ingredient
            {
                Name = name,
                Price = price,
                Picture = null // 음료는 이미지 없음
            });

            totalPrice += price;

            if (ingredientCount.ContainsKey(name))
                ingredientCount[name]++;
            else
                ingredientCount[name] = 1;

            UpdatePriceLabel();
            UpdateSeparateIngredientLists();
        }

        private void bt_Cola_Click(object sender, EventArgs e)
        {
            AddDrink("코카콜라", 3000);
        }

        private void bt_Orange_Click(object sender, EventArgs e)
        {
            AddDrink("오렌지주스", 3000);
        }

        private void bt_Cider_Click(object sender, EventArgs e)
        {
            AddDrink("스프라이트", 3000);
        }


        void AddSide(string name, int price)
        {
            selectedIngredients.Add(new Ingredient
            {
                Name = name,
                Price = price,
                Picture = null  // 사이드는 이미지 없음
            });

            if (ingredientCount.ContainsKey(name))
                ingredientCount[name]++;
            else
                ingredientCount[name] = 1;

            totalPrice += price;

            UpdatePriceLabel();
            UpdateSeparateIngredientLists();
        }
        private void bt_Fries_Click(object sender, EventArgs e)
        {
            AddSide("감자튀김", 5000);
        }

        private void bt_Nugget_Click(object sender, EventArgs e)
        {
            AddSide("치킨너겟", 5000);
        }

        private void bt_CheeseStick_Click(object sender, EventArgs e)
        {
            AddSide("치즈스틱", 4000);
        }

        private void bt_Reset_Click(object sender, EventArgs e)
        {
            // 재료 이미지 제거
            foreach (var ing in selectedIngredients)
            {
                if (ing.Picture != null)
                    pl_Burger.Controls.Remove(ing.Picture);
            }

            selectedIngredients.Clear();
            ingredientCount.Clear();
            totalPrice = 0;

            // 번 이미지 초기화
            topBun.Image = null;
            bottomBun.Image = null;
            pl_Menu.Visible = true;
            pl_Drink.Visible = false;
            pl_Side.Visible = false;
            pl_OrderBurger.Visible = false;
            pl_Payment.Visible = false;

            otheringre = 0;
            // 라벨, 리스트 등 초기화
            UpdatePriceLabel();
            UpdateSeparateIngredientLists();
        }

        private void bt_Payment_Click(object sender, EventArgs e)
        {
            lstSummary.Items.Clear();
            var summary = GetFormattedIngredientSummary();

            foreach (var line in summary)
                lstSummary.Items.Add(line);

            lb_FinalPrice.Text = $"총 결제 금액: {totalPrice}원";

            //결제 패널만 표시
            pl_Side.Visible = false;
            pl_Payment.Visible = true;
            plSideButton.Visible = false;
        }

        private void bt_ConfirmPay_Click(object sender, EventArgs e)
        {
            if (lstSummary.Items.Count == 0)
            {
                MessageBox.Show("결제할 항목이 없습니다");
            }
            else
            {
                MessageBox.Show("결제가 완료되었습니다!");

                StringBuilder receipt = new StringBuilder();

                // 현재 시간
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (takeout == 1)
                {
                    receipt.AppendLine("----------------------------------");
                    receipt.AppendLine("            포장 주문             ");
                    receipt.AppendLine("----------------------------------");
                }
                else
                {
                    receipt.AppendLine("----------------------------------------");
                    receipt.AppendLine("            매장 주문             ");
                    receipt.AppendLine("----------------------------------------");
                }
                // 매장 정보
                receipt.AppendLine("★ SONG's BURGER 키오스크 ★");
                receipt.AppendLine("충남 천안시 동남구 대흥로 255 화일빌딩");
                receipt.AppendLine("                사업자 : 송상현");
                receipt.AppendLine("대표전화: 041-123-1234");
                receipt.AppendLine("----------------------------------------");
                receipt.AppendLine($"주문 시간: {time}");
                receipt.AppendLine("----------------------------------------");
                receipt.AppendLine("메뉴       수량    금액");
                receipt.AppendLine("----------------------------------------");

                // 주문 목록
                foreach (var item in lstSummary.Items)
                {
                    //"버거 x2  6000원"
                    receipt.AppendLine(item.ToString());
                }

                receipt.AppendLine("----------------------------------------");
                receipt.AppendLine($"총 결제금액:      {totalPrice:N0}원");
                receipt.AppendLine("----------------------------------------");
                receipt.AppendLine("현금영수증 발급은 카운터에 문의해주세요");
                receipt.AppendLine("감사합니다! 좋은 하루 되세요 :)");

                // 출력
                MessageBox.Show(receipt.ToString(), "영수증", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 전체 초기화
                ClearAll();
                pl_Payment.Visible = false;
                pl_Side.Visible = false;
                pl_Menu.Visible = true;
                takeout = 0;
            }
        }
        void ClearAll()
        {
            foreach (var ing in selectedIngredients)
            {
                if (ing.Picture != null)
                    pl_Burger.Controls.Remove(ing.Picture);
            }

            selectedIngredients.Clear();
            ingredientCount.Clear();
            totalPrice = 0;

            UpdatePriceLabel();
            UpdateSeparateIngredientLists();

            // 번 이미지도 초기화
            topBun.Image = null;
            bottomBun.Image = null;
            otheringre = 0;
        }

        private void bt_CancelPay_Click(object sender, EventArgs e)
        {
            // 다시 주문화면으로 돌아감
            pl_Payment.Visible = false;
            pl_OrderBurger.Visible = true;
            plBurgerButton.Visible = true;
        }


        void UpdateSeparateIngredientLists()
        {
            lstBurger.Items.Clear();
            lstDrink.Items.Clear();
            lstSide.Items.Clear();

            var 재료 = ingredientCount.Where(i => !IsDrink(i.Key) && !IsSide(i.Key));
            var 음료 = ingredientCount.Where(i => IsDrink(i.Key));
            var 사이드 = ingredientCount.Where(i => IsSide(i.Key));

            foreach (var item in 재료)
                lstBurger.Items.Add($"{item.Key}: {item.Value}개");

            foreach (var item in 음료)
                lstDrink.Items.Add($"{item.Key}: {item.Value}개");

            foreach (var item in 사이드)
                lstSide.Items.Add($"{item.Key}: {item.Value}개");
        }

        private void lstDrink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrink.SelectedItem == null) return;

            string selectedText = lstDrink.SelectedItem.ToString();
            string name = selectedText.Split(':')[0].Trim();

            if (ingredientCount.ContainsKey(name))
            {
                ingredientCount[name]--;
                if (ingredientCount[name] <= 0)
                    ingredientCount.Remove(name);

                // 가격도 반영해서 줄이기
                var matched = selectedIngredients.FirstOrDefault(i => i.Name == name && i.Picture == null); // 음료/사이드라 이미지 없음
                if (matched != null)
                {
                    selectedIngredients.Remove(matched);
                    totalPrice -= matched.Price;
                }

                UpdatePriceLabel();
                UpdateSeparateIngredientLists();
            }
        }

        private void lstSide_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSide.SelectedItem == null) return;

            string selectedText = lstSide.SelectedItem.ToString();
            string name = selectedText.Split(':')[0].Trim();

            if (ingredientCount.ContainsKey(name))
            {
                ingredientCount[name]--;
                if (ingredientCount[name] <= 0)
                    ingredientCount.Remove(name);

                // 가격도 반영해서 줄이기
                var matched = selectedIngredients.FirstOrDefault(i => i.Name == name && i.Picture == null); // 음료/사이드라 이미지 없음
                if (matched != null)
                {
                    selectedIngredients.Remove(matched);
                    totalPrice -= matched.Price;
                }

                UpdatePriceLabel();
                UpdateSeparateIngredientLists();
            }
        }

        private void bt_Zcola_Click(object sender, EventArgs e)
        {
            AddDrink("팹시제로", 3000);
        }

        private void bt_Drp_Click(object sender, EventArgs e)
        {
            AddDrink("닥터페퍼", 3000);
        }

        private void bt_Americano_Click(object sender, EventArgs e)
        {
            AddDrink("아메리카노", 3000);
        }

        private void bt_Sparkling_Click(object sender, EventArgs e)
        {
            AddDrink("탄산수", 2000);
        }

        private void bt_Water_Click(object sender, EventArgs e)
        {
            AddDrink("삼다수", 1000);
        }

        private void bt_FriesW_Click(object sender, EventArgs e)
        {
            AddSide("웨지감자", 5000);
        }

        private void bt_Salad_Click(object sender, EventArgs e)
        {
            AddSide("샐러드", 5000);
        }

        private void bt_OnionRing_Click(object sender, EventArgs e)
        {
            AddSide("어니언링", 5000);
        }

        private void bt_Ssauce_Click(object sender, EventArgs e)
        {
            AddSide("특제소스", 500);
        }

        private void bt_NextDrink_Click(object sender, EventArgs e)
        {
            plBurgerButton.Visible = false;
            plDrinkButton.Visible = true;
            pl_OrderBurger.Visible = false;
            pl_Drink.Visible = true;
        }

        private void bt_BackBurger_Click(object sender, EventArgs e)
        {
            plDrinkButton.Visible = false;
            plBurgerButton.Visible = true;
            pl_Drink.Visible = false;
            pl_OrderBurger.Visible = true;
        }

        private void bt_NextSide_Click(object sender, EventArgs e)
        {
            plDrinkButton.Visible = false;
            plSideButton.Visible = true;
            pl_Drink.Visible = false;
            pl_Side.Visible = true;
        }

        private void bt_BackDrink_Click(object sender, EventArgs e)
        {
            plSideButton.Visible = false;
            plDrinkButton.Visible = true;
            pl_Side.Visible = false;
            pl_Drink.Visible = true;
        }

        private void bt_Reset1_Click(object sender, EventArgs e)
        {
            // 재료 이미지 제거
            foreach (var ing in selectedIngredients)
            {
                if (ing.Picture != null)
                    pl_Burger.Controls.Remove(ing.Picture);
            }

            selectedIngredients.Clear();
            ingredientCount.Clear();
            totalPrice = 0;

            // 번 이미지 초기화
            topBun.Image = null;
            bottomBun.Image = null;
            pl_Menu.Visible = true;
            pl_Drink.Visible = false;
            pl_Side.Visible = false;
            pl_OrderBurger.Visible = false;
            pl_Payment.Visible = false;

            otheringre = 0;
            // 라벨, 리스트 등 초기화
            UpdatePriceLabel();
            UpdateSeparateIngredientLists();
        }
        List<Image> adBannerImages = new List<Image>();

        void LoadAdImages()
        {
            adBannerImages.Clear();
            adBannerImages.Add(Image.FromFile("Images/Ad1.png"));
            adBannerImages.Add(Image.FromFile("Images/Ad2.png"));
            adBannerImages.Add(Image.FromFile("Images/Ad3.png"));
        }

        int currentAdIndex = 0;

        void ShowNextAd()
        {
            if (adBannerImages.Count == 0) return;

            currentAdIndex = (currentAdIndex + 1) % adBannerImages.Count;
            pb_AD.Image = adBannerImages[currentAdIndex];
            pb_AD.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowNextAd();
        }


        void LoadGifAd()
        {
            pictureBox1.Image = Image.FromFile("Images/tropi.gif"); // gif 경로
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}





