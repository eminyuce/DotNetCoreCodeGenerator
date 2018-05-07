using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Entities
{
    public class NwmTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
        public class NwmCurrencyConfig
    {
        public int currency_config_id { get; set; }
        public float buy_at_price { get; set; }
        public float buy_on_percent { get; set; }
        public float order_timeout_in_hour { get; set; }
        public Boolean buyable { get; set; }
        public string currency_pair { get; set; }
        public float sell_at_price { get; set; }
        public float sell_on_percent { get; set; }
        public Boolean sellable { get; set; }
        public float usable_balance_percent { get; set; }
        public int bot_user { get; set; }
    }
    public class NwmAyarlar
    {
        public int id { get; set; }
        public string siteBasligi { get; set; }
        public string anahtarKelimeler { get; set; }
        public string google { get; set; }
        public string siteAciklamasi { get; set; }
        public string firmaAdi { get; set; }
        public string telefon { get; set; }
        public string telefon2 { get; set; }
        public string faks { get; set; }
        public string eposta { get; set; }
        public string adres { get; set; }
        public string yetkili { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int logKayit { get; set; }
        public int durum { get; set; }
        public string siteUrl { get; set; }
        public string panelUrl { get; set; }
        public string smtpSunucu { get; set; }
        public string smtpPort { get; set; }
        public string smtpKullanici { get; set; }
        public string smtpSifre { get; set; }
        public string smtpAd { get; set; }
        public string smtpMetod { get; set; }
        public string smtpDurum { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string gplus { get; set; }
        public string foursquare { get; set; }
        public string map { get; set; }
    }
    public class NwmHaberler
    {
        public int id { get; set; }
        public int sira { get; set; }
        public DateTime tarih { get; set; }
        public int durum { get; set; }
        public string seo { get; set; }
        public string link { get; set; }
        public string baslik_tr { get; set; }
        public string keywords_tr { get; set; }
        public string ozet_tr { get; set; }
        public string detay_tr { get; set; }
        public string baslik_en { get; set; }
        public string keywords_en { get; set; }
        public string ozet_en { get; set; }
        public string detay_en { get; set; }
        public DateTime haberTarihi { get; set; }
        public string baslik_de { get; set; }
        public string keywords_de { get; set; }
        public string ozet_de { get; set; }
        public string detay_de { get; set; }
        public int tip { get; set; }
        public string baslik_ar { get; set; }
        public string keywords_ar { get; set; }
        public string ozet_ar { get; set; }
        public string detay_ar { get; set; }
        public string yorum_tr { get; set; }
        public string yorum_en { get; set; }
    }
    }
