#include <Arduino.h>

#define LED 13
#define REQUEST_MSG 'G'
#define RETURN_ERR_MSG "Wrong request message!"
#define CHAR_RANGE_SIZE 62
#define RANDOM_STR_LTH 15
#define RAND_STR_BUFF_LTH (RANDOM_STR_LTH + 1)
#define BAUD_RATE 9600
#define WAITING_DELAY 3

const char charRange[CHAR_RANGE_SIZE] = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
char randStr[RAND_STR_BUFF_LTH];
char received;
char inputTrash;

char* generateRandomStr(char* randBuff, size_t buffSize);

void setup() 
{
  pinMode(LED,OUTPUT);
  digitalWrite(LED,LOW);

  Serial.begin(BAUD_RATE);

  randomSeed(analogRead(0));
}

void loop() 
{
  received = Serial.read();

  switch(received)
  {
    case REQUEST_MSG:
      digitalWrite(LED,HIGH);
      Serial.print(generateRandomStr(randStr,RAND_STR_BUFF_LTH));
      digitalWrite(LED,LOW);
    break;

    case -1:
    break;

    default:
      Serial.print(RETURN_ERR_MSG);
      //clearing rest of the buffer
      while(true)
      {
        //waiting for rest of data to arrive
        delay(WAITING_DELAY);

        //clearing information, if received
        if(Serial.available())
        {
          while(Serial.available())
            Serial.read();
        }
        else
          //nothing arrived
          break;
      }
    break;
  }
}


char* generateRandomStr(char* randBuff, size_t buffSize)
{
  for(size_t i=0; i<buffSize-1; ++i)
  {
    randBuff[i] = charRange[random(0,CHAR_RANGE_SIZE)];
  }

  randBuff[buffSize-1] = '\0';

  return randBuff;
}
