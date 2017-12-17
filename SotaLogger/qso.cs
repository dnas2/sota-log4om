using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SotaLogger
{
    class qso
    {
        public string callsign;
        public DateTime timeOn;
        public int rstTx;
        public int rstRx;
        public string mode;
        public string equipmentString;
        public string myCall;
        public int power;
        public float band;
        public double frequency;
        public string sotaRef;
        public string sotaRefRx;
        public string grid;

        internal void parseEquipmentString()
        {
            int splitPoint = equipmentString.IndexOf("W ");
            power = int.Parse(equipmentString.Substring(0, splitPoint));
            string bandStr = equipmentString.Substring((splitPoint + 2));
            bandStr = bandStr.Replace("m","");
            if (bandStr == "70cs") { bandStr = "0.7"; };
            band = float.Parse(bandStr);
        }

        internal void setFrequencyFromBandMode()
        {
            double tmpFreq = 0;
            switch (band.ToString())
            {
                case "60": tmpFreq = 5.3; break;
                case "40":
                    switch (mode)
                    {
                        case "CW": tmpFreq = 7.03; break;
                        case "SSB":
                            tmpFreq = 7.15; break;
                        default:
                            tmpFreq = 7.1; break;
                    }
                    break;
                case "30": tmpFreq = 10.1; break;
                case "20":
                    switch (mode)
                    {
                        case "CW": tmpFreq = 14.03; break;
                        case "SSB":
                            tmpFreq = 14.2; break;
                        default:
                            tmpFreq = 14.1; break;
                    }
                    break;
                case "17":
                    switch (mode)
                    {
                        case "CW": tmpFreq = 18.07; break;
                        case "SSB":
                            tmpFreq = 18.1; break;
                        default:
                            tmpFreq = 18.09; break;
                    }
                    break;
                case "6":
                    switch (mode)
                    {
                        case "CW": tmpFreq = 50.1; break;
                        case "SSB":
                            tmpFreq = 50.3; break;
                        case "FM":
                            tmpFreq = 513; break;
                        default:
                            tmpFreq = 7.1; break;
                    }
                    break;
                case "2":
                    switch (mode)
                    {
                        case "CW": tmpFreq = 144.1; break;
                        case "SSB":
                            tmpFreq = 144.3; break;
                        case "FM":
                            tmpFreq = 145.5; break;
                        default:
                            tmpFreq = 145; break;
                    }
                    break;
                case "0.7":
                    switch (mode)
                    {
                        case "CW": tmpFreq = 432.1; break;
                        case "SSB":
                            tmpFreq = 432.3; break;
                        case "FM":
                            tmpFreq = 433.5; break;
                        default:
                            tmpFreq = 432; break;
                    }
                    break;

            }
            frequency = tmpFreq;
        }

        internal string AddToMySqlString()
        {
            string retVal = "INSERT INTO `log` (QsoId, `Call`, Band, Freq,`Mode`, QSODate, TimeOn,MyGridSquare,MyName,TheOperator,OwnerCallsign,StationCallsign,RstRcvd,RstSent,TimeOff,Portable,TxPwrDecimal,MySotaRef,SotaRef,QslMsg) VALUES (";
            retVal += "" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ", ";
            retVal += "'" + callsign + "', ";
            if (band > 1)
            {
                retVal += "'" + band + "m', "; 
            }
            else
            {
                retVal += "'" + (band*100) + "cm', ";
            }
            retVal += "'" + (frequency*1000).ToString() + "', "; 
            retVal += "'" + mode + "', ";
            retVal += "" + timeOn.ToString("yyyyMMdd") + ", ";
            retVal += "'" + timeOn.ToString("HHmmss") + "', ";
            retVal += "'" + grid + "', ";
            retVal += "'Dom', 'M0BLF', 'M0BLF', ";
            retVal += "'" + myCall + "', ";
            retVal += "" + rstRx + ", ";
            retVal += "" + rstTx + ", ";
            retVal += "'" + timeOn.ToString("HHmmss") + "', ";
            retVal += "true, ";
            retVal += "" + power + ", ";
            retVal += "'" + sotaRef + "', ";
            retVal += "'" + sotaRefRx + "', ";
            retVal += "'My QTH SOTA: " + sotaRef + " (" + grid + ")'); ";
            return retVal;
        }

        internal string AddToCsvExport()
        {
            string retVal = "V2," + myCall + "," + sotaRef + "," + timeOn.ToString("dd/MM/yy,HHmm") + "," + frequency + "MHz," + mode + "," + callsign;
            if (sotaRefRx != null && sotaRefRx.Length>1)
            {
                retVal += "," + sotaRefRx;
            }
            return retVal;
        }
    }
}
