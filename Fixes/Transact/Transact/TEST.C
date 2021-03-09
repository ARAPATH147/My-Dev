int atoi(char* num);

typedef struct TEST_Struct {
    int somefield;
    char name[3];
} TEST;

void pants(void) {
    TEST test1;
    TEST *t1 = &test1;
    int temp = atoi(t1->name);
}
